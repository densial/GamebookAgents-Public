using System.Text.Json;

namespace GamebookAgents.RAG;

public sealed class RagStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly string _indexPath;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public RagStore(string indexPath)
    {
        _indexPath = Path.GetFullPath(
            string.IsNullOrWhiteSpace(indexPath)
                ? throw new ArgumentException("An index path is required.", nameof(indexPath))
                : indexPath);
    }

    public string IndexPath => _indexPath;

    public async Task<IndexedDocument?> GetDocumentAsync(
        string sourcePath,
        CancellationToken cancellationToken = default)
    {
        var normalizedPath = NormalizePath(sourcePath);
        var index = await ReadAsync(cancellationToken);
        return index.Documents.SingleOrDefault(
            document => PathEquals(document.SourcePath, normalizedPath));
    }

    public async Task<IReadOnlyList<DocumentChunk>> GetChunksAsync(
        CancellationToken cancellationToken = default)
    {
        var index = await ReadAsync(cancellationToken);
        return index.Chunks;
    }

    public async Task ReplaceDocumentAsync(
        IndexedDocument document,
        IReadOnlyList<DocumentChunk> chunks,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(chunks);

        if (chunks.Count == 0)
        {
            throw new RagStoreException("Cannot store an indexed document without chunks.");
        }

        if (chunks.Any(chunk => chunk.Embedding.Length != document.EmbeddingDimension))
        {
            throw new RagStoreException(
                $"One or more chunks do not match embedding dimension {document.EmbeddingDimension}.");
        }

        await _gate.WaitAsync(cancellationToken);
        try
        {
            var index = await LoadWithoutLockAsync(cancellationToken);
            var otherDocuments = index.Documents
                .Where(item => !PathEquals(item.SourcePath, document.SourcePath))
                .ToArray();

            if (otherDocuments.Length > 0
                && (!string.Equals(index.EmbeddingModel, document.EmbeddingModel, StringComparison.Ordinal)
                    || index.EmbeddingDimension != document.EmbeddingDimension))
            {
                throw new RagStoreException(
                    $"The existing index uses model '{index.EmbeddingModel}' with dimension "
                    + $"{index.EmbeddingDimension}, but the new chunks use model "
                    + $"'{document.EmbeddingModel}' with dimension {document.EmbeddingDimension}. "
                    + "Re-index all configured documents after changing embedding models.");
            }

            index.Documents.RemoveAll(item => PathEquals(item.SourcePath, document.SourcePath));
            index.Chunks.RemoveAll(item => PathEquals(item.SourcePath, document.SourcePath));
            index.Documents.Add(document);
            index.Chunks.AddRange(chunks);
            index.EmbeddingModel = document.EmbeddingModel;
            index.EmbeddingDimension = document.EmbeddingDimension;

            await SaveWithoutLockAsync(index, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<RagIndex> ReadAsync(CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            return await LoadWithoutLockAsync(cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<RagIndex> LoadWithoutLockAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_indexPath))
        {
            return new RagIndex();
        }

        try
        {
            await using var stream = File.OpenRead(_indexPath);
            var index = await JsonSerializer.DeserializeAsync<RagIndex>(stream, JsonOptions, cancellationToken)
                        ?? throw new RagStoreException($"RAG index is empty: {_indexPath}");
            Validate(index);
            return index;
        }
        catch (RagStoreException)
        {
            throw;
        }
        catch (JsonException exception)
        {
            throw new RagStoreException(
                $"RAG index contains invalid JSON: {_indexPath}. {exception.Message}",
                exception);
        }
        catch (IOException exception)
        {
            throw new RagStoreException(
                $"Could not read RAG index: {_indexPath}. {exception.Message}",
                exception);
        }
    }

    private async Task SaveWithoutLockAsync(RagIndex index, CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(_indexPath)
                        ?? throw new RagStoreException($"Index path has no parent directory: {_indexPath}");
        Directory.CreateDirectory(directory);
        var temporaryPath = $"{_indexPath}.{Guid.NewGuid():N}.tmp";

        try
        {
            await using (var stream = new FileStream(
                             temporaryPath,
                             FileMode.CreateNew,
                             FileAccess.Write,
                             FileShare.None,
                             81920,
                             FileOptions.Asynchronous))
            {
                await JsonSerializer.SerializeAsync(stream, index, JsonOptions, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }

            File.Move(temporaryPath, _indexPath, overwrite: true);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new RagStoreException(
                $"Could not write RAG index: {_indexPath}. {exception.Message}",
                exception);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    private void Validate(RagIndex index)
    {
        if (index.Version != RagIndex.CurrentVersion)
        {
            throw new RagStoreException(
                $"Unsupported RAG index version {index.Version} in {_indexPath}; "
                + $"expected {RagIndex.CurrentVersion}.");
        }

        if (index.Documents is null || index.Chunks is null)
        {
            throw new RagStoreException($"RAG index is missing required collections: {_indexPath}");
        }

        foreach (var chunk in index.Chunks)
        {
            if (string.IsNullOrWhiteSpace(chunk.SourcePath)
                || string.IsNullOrWhiteSpace(chunk.Text)
                || chunk.Embedding is null
                || chunk.Embedding.Length == 0)
            {
                throw new RagStoreException($"RAG index contains an invalid chunk: {_indexPath}");
            }

            if (index.EmbeddingDimension > 0 && chunk.Embedding.Length != index.EmbeddingDimension)
            {
                throw new RagStoreException(
                    $"RAG index contains a chunk with dimension {chunk.Embedding.Length}; "
                    + $"expected {index.EmbeddingDimension}: {_indexPath}");
            }

            if (chunk.Embedding.Any(value => !float.IsFinite(value)))
            {
                throw new RagStoreException($"RAG index contains a non-finite embedding: {_indexPath}");
            }
        }

        foreach (var document in index.Documents)
        {
            var actualChunkCount = index.Chunks.Count(chunk => PathEquals(
                chunk.SourcePath,
                document.SourcePath));

            if (document.ChunkCount <= 0 || document.ChunkCount != actualChunkCount)
            {
                throw new RagStoreException(
                    $"RAG index chunk count is inconsistent for {document.SourceFilename}: {_indexPath}");
            }
        }
    }

    private static string NormalizePath(string path) => Path.GetFullPath(path);

    private static bool PathEquals(string left, string right) =>
        string.Equals(NormalizePath(left), NormalizePath(right), StringComparison.OrdinalIgnoreCase);
}
