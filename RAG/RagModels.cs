namespace GamebookAgents.RAG;

public sealed class IndexedDocument
{
    public string SourcePath { get; set; } = string.Empty;

    public string SourceFilename { get; set; } = string.Empty;

    public string SourceHash { get; set; } = string.Empty;

    public string EmbeddingModel { get; set; } = string.Empty;

    public int EmbeddingDimension { get; set; }

    public int ChunkSizeWords { get; set; }

    public int ChunkOverlapWords { get; set; }

    public int ChunkCount { get; set; }

    public DateTimeOffset IndexedAtUtc { get; set; }
}

public sealed class RagIndex
{
    public const int CurrentVersion = 1;

    public int Version { get; set; } = CurrentVersion;

    public string EmbeddingModel { get; set; } = string.Empty;

    public int EmbeddingDimension { get; set; }

    public List<IndexedDocument> Documents { get; set; } = [];

    public List<DocumentChunk> Chunks { get; set; } = [];
}

public sealed record RagSearchResult(
    string SourceFilename,
    string SourcePath,
    int? PageNumber,
    int ChunkNumber,
    string? Section,
    string Text,
    double SimilarityScore);

public sealed record DocumentIndexingResult(
    string SourcePath,
    int ChunkCount,
    bool WasAlreadyIndexed);

internal sealed record SourceTextSegment(
    string Text,
    int? PageNumber,
    bool RecognizeMarkdownHeadings);

internal sealed record ChunkDraft(
    string Text,
    int? PageNumber,
    string? Section);
