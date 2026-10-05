namespace GamebookAgents.RAG;

public sealed class DocumentChunk
{
    public string Id { get; set; } = string.Empty;

    public string SourcePath { get; set; } = string.Empty;

    public string SourceFilename { get; set; } = string.Empty;

    public int? PageNumber { get; set; }

    public int ChunkNumber { get; set; }

    public string? Section { get; set; }

    public string Text { get; set; } = string.Empty;

    public float[] Embedding { get; set; } = [];
}
