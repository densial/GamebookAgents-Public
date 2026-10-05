namespace GamebookAgents.RAG;

public sealed class RagSearchService
{
    private readonly EmbeddingService _embeddingService;
    private readonly RagStore _store;

    public RagSearchService(EmbeddingService embeddingService, RagStore store)
    {
        _embeddingService = embeddingService;
        _store = store;
    }

    public async Task<IReadOnlyList<RagSearchResult>> SearchAsync(
        string query,
        int topK = 6,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("A non-empty reference query is required.", nameof(query));
        }

        if (topK < 1 || topK > 20)
        {
            throw new ArgumentOutOfRangeException(nameof(topK), "topK must be between 1 and 20.");
        }

        var chunks = await _store.GetChunksAsync(cancellationToken);
        if (chunks.Count == 0)
        {
            throw new RagStoreException(
                $"The RAG index contains no chunks. Index reference documents first: {_store.IndexPath}");
        }

        var queryEmbedding = await _embeddingService.EmbedQueryAsync(query, cancellationToken);

        return chunks
            .Select(chunk => new RagSearchResult(
                chunk.SourceFilename,
                chunk.SourcePath,
                chunk.PageNumber,
                chunk.ChunkNumber,
                chunk.Section,
                chunk.Text,
                CosineSimilarity(queryEmbedding, chunk.Embedding)))
            .OrderByDescending(result => result.SimilarityScore)
            .ThenBy(result => result.SourcePath, StringComparer.OrdinalIgnoreCase)
            .ThenBy(result => result.ChunkNumber)
            .Take(topK)
            .ToArray();
    }

    public static double CosineSimilarity(ReadOnlySpan<float> left, ReadOnlySpan<float> right)
    {
        if (left.Length != right.Length)
        {
            throw new RagException(
                $"Embedding dimension mismatch: query has {left.Length} values and chunk has {right.Length}.");
        }

        if (left.Length == 0)
        {
            throw new RagException("Cannot compare empty embeddings.");
        }

        double dotProduct = 0;
        double leftMagnitudeSquared = 0;
        double rightMagnitudeSquared = 0;

        for (var index = 0; index < left.Length; index++)
        {
            dotProduct += left[index] * right[index];
            leftMagnitudeSquared += left[index] * left[index];
            rightMagnitudeSquared += right[index] * right[index];
        }

        if (leftMagnitudeSquared == 0 || rightMagnitudeSquared == 0)
        {
            return 0;
        }

        return dotProduct / (Math.Sqrt(leftMagnitudeSquared) * Math.Sqrt(rightMagnitudeSquared));
    }
}
