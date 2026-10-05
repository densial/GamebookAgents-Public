using System.Security.Cryptography;
using System.Text;
using GamebookAgents.Configuration;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace GamebookAgents.RAG;

public sealed class DocumentIndexer
{
    private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".md", ".markdown", ".txt"
    };

    private readonly string _projectRoot;
    private readonly EmbeddingService _embeddingService;
    private readonly TextChunker _chunker;
    private readonly RagStore _store;
    private readonly RagOptions _options;

    public DocumentIndexer(
        string projectRoot,
        EmbeddingService embeddingService,
        TextChunker chunker,
        RagStore store,
        RagOptions options)
    {
        _projectRoot = Path.GetFullPath(projectRoot);
        _embeddingService = embeddingService;
        _chunker = chunker;
        _store = store;
        _options = options;
    }

    public async Task<IReadOnlyList<DocumentIndexingResult>> IndexConfiguredDocumentsAsync(
        CancellationToken cancellationToken = default)
    {
        var results = new List<DocumentIndexingResult>();

        foreach (var path in _options.ResolveReferenceDocuments(_projectRoot))
        {
            results.Add(await IndexAsync(path, cancellationToken));
        }

        return results;
    }

    public async Task<DocumentIndexingResult> IndexAsync(
        string sourcePath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sourcePath))
        {
            throw new ArgumentException("A reference document path is required.", nameof(sourcePath));
        }

        var fullPath = Path.GetFullPath(sourcePath, _projectRoot);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Reference document does not exist: {fullPath}", fullPath);
        }

        var extension = Path.GetExtension(fullPath);
        if (!SupportedExtensions.Contains(extension))
        {
            throw new NotSupportedException(
                $"Unsupported reference document type '{extension}' for {fullPath}. "
                + "Supported types are PDF, Markdown, and plain text.");
        }

        var sourceHash = await ComputeHashAsync(fullPath, cancellationToken);
        var existingDocument = await _store.GetDocumentAsync(fullPath, cancellationToken);

        if (!NeedsReindex(
                existingDocument,
                sourceHash,
                _embeddingService.Model,
                _options.ChunkSizeWords,
                _options.ChunkOverlapWords))
        {
            Console.WriteLine($"Already indexed: {Path.GetFileName(fullPath)}");
            return new DocumentIndexingResult(fullPath, existingDocument!.ChunkCount, true);
        }

        Console.WriteLine($"Indexing {Path.GetFileName(fullPath)}");
        var segments = await ExtractAsync(fullPath, extension, cancellationToken);
        var drafts = segments.SelectMany(_chunker.Chunk).ToArray();

        if (drafts.Length == 0)
        {
            throw new RagException(
                $"Reference document contains no extractable text: {fullPath}. "
                + "Scanned PDFs require OCR before indexing.");
        }

        var embeddings = await _embeddingService.EmbedDocumentsAsync(
            drafts.Select(draft => draft.Text).ToArray(),
            cancellationToken);

        if (embeddings.Count != drafts.Length)
        {
            throw new EmbeddingServiceException(
                $"Embedding count mismatch for {fullPath}: expected {drafts.Length}, got {embeddings.Count}.");
        }

        var dimension = embeddings[0].Length;
        var sourceFilename = Path.GetFileName(fullPath);
        var chunks = drafts.Select((draft, index) => new DocumentChunk
        {
            Id = CreateChunkId(fullPath, sourceHash, index + 1),
            SourcePath = fullPath,
            SourceFilename = sourceFilename,
            PageNumber = draft.PageNumber,
            ChunkNumber = index + 1,
            Section = draft.Section,
            Text = draft.Text,
            Embedding = embeddings[index]
        }).ToArray();

        var document = new IndexedDocument
        {
            SourcePath = fullPath,
            SourceFilename = sourceFilename,
            SourceHash = sourceHash,
            EmbeddingModel = _embeddingService.Model,
            EmbeddingDimension = dimension,
            ChunkSizeWords = _options.ChunkSizeWords,
            ChunkOverlapWords = _options.ChunkOverlapWords,
            ChunkCount = chunks.Length,
            IndexedAtUtc = DateTimeOffset.UtcNow
        };

        // Replacement happens only after all extraction and embedding work succeeds,
        // so a transient LM Studio failure cannot destroy the previous usable version.
        await _store.ReplaceDocumentAsync(document, chunks, cancellationToken);
        Console.WriteLine($"Indexed {chunks.Length} chunks from {sourceFilename}");

        return new DocumentIndexingResult(fullPath, chunks.Length, false);
    }

    internal static bool NeedsReindex(
        IndexedDocument? existingDocument,
        string sourceHash,
        string embeddingModel,
        int chunkSizeWords,
        int chunkOverlapWords)
    {
        return existingDocument is null
               || !string.Equals(existingDocument.SourceHash, sourceHash, StringComparison.OrdinalIgnoreCase)
               || !string.Equals(existingDocument.EmbeddingModel, embeddingModel, StringComparison.Ordinal)
               || existingDocument.ChunkSizeWords != chunkSizeWords
               || existingDocument.ChunkOverlapWords != chunkOverlapWords;
    }

    public static async Task<string> ComputeHashAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                81920,
                FileOptions.Asynchronous | FileOptions.SequentialScan);
            var hash = await SHA256.HashDataAsync(stream, cancellationToken);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new RagException($"Could not hash reference document {path}. {exception.Message}", exception);
        }
    }

    private static async Task<IReadOnlyList<SourceTextSegment>> ExtractAsync(
        string path,
        string extension,
        CancellationToken cancellationToken)
    {
        try
        {
            if (extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return ExtractPdf(path, cancellationToken);
            }

            var text = await File.ReadAllTextAsync(path, cancellationToken);
            return
            [
                new SourceTextSegment(
                    text,
                    null,
                    extension.Equals(".md", StringComparison.OrdinalIgnoreCase)
                    || extension.Equals(".markdown", StringComparison.OrdinalIgnoreCase))
            ];
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new RagException(
                $"Could not extract text from reference document {path}. {exception.Message}",
                exception);
        }
    }

    private static IReadOnlyList<SourceTextSegment> ExtractPdf(
        string path,
        CancellationToken cancellationToken)
    {
        var segments = new List<SourceTextSegment>();
        using var document = PdfDocument.Open(path);

        for (var pageNumber = 1; pageNumber <= document.NumberOfPages; pageNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var text = ContentOrderTextExtractor.GetText(document.GetPage(pageNumber));

            if (!string.IsNullOrWhiteSpace(text))
            {
                segments.Add(new SourceTextSegment(text, pageNumber, false));
            }
        }

        return segments;
    }

    private static string CreateChunkId(
        string sourcePath,
        string sourceHash,
        int chunkNumber)
    {
        var identity = Encoding.UTF8.GetBytes($"{sourcePath}\n{sourceHash}\n{chunkNumber}");
        return Convert.ToHexString(SHA256.HashData(identity)).ToLowerInvariant();
    }
}
