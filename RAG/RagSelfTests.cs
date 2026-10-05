namespace GamebookAgents.RAG;

public static class RagSelfTests
{
    public static async Task RunAsync(CancellationToken cancellationToken = default)
    {
        TestCosineSimilarity();
        TestChunking();
        TestReindexDecision();
        await TestStoreReplacementAsync(cancellationToken);
        Console.WriteLine("RAG self-tests passed: cosine similarity, chunking, unchanged detection, and document replacement.");
    }

    private static void TestCosineSimilarity()
    {
        AssertAlmostEqual(1, RagSearchService.CosineSimilarity([1, 2], [1, 2]), "identical vectors");
        AssertAlmostEqual(0, RagSearchService.CosineSimilarity([1, 0], [0, 1]), "orthogonal vectors");
        AssertAlmostEqual(-1, RagSearchService.CosineSimilarity([1, 0], [-1, 0]), "opposite vectors");
        AssertAlmostEqual(0, RagSearchService.CosineSimilarity([0, 0], [1, 0]), "zero vector");
    }

    private static void TestChunking()
    {
        var text = string.Join(' ', Enumerable.Range(1, 25).Select(number => $"w{number}"));
        var chunker = new TextChunker(10, 2);
        var chunks = chunker.Chunk(new SourceTextSegment(text, 4, false));

        Assert(chunks.Count == 3, $"expected 3 chunks but got {chunks.Count}");
        Assert(chunks.All(chunk => chunk.PageNumber == 4), "page metadata was not retained");
        Assert(chunks[0].Text.EndsWith("w9 w10", StringComparison.Ordinal), "first chunk boundary is wrong");
        Assert(chunks[1].Text.StartsWith("w9 w10", StringComparison.Ordinal), "chunk overlap is missing");

        var markdownChunks = chunker.Chunk(new SourceTextSegment(
            "# Grappling\n\nRules for taking hold of a creature.\n\n## Escaping\n\nRules for escaping the grapple.",
            null,
            true));
        Assert(markdownChunks.Any(chunk => chunk.Section == "Grappling"), "Markdown heading was not retained");
        Assert(markdownChunks.Any(chunk => chunk.Section == "Escaping"), "nested Markdown heading was not retained");
    }

    private static void TestReindexDecision()
    {
        var document = new IndexedDocument
        {
            SourceHash = "same-hash",
            EmbeddingModel = "model",
            ChunkSizeWords = 350,
            ChunkOverlapWords = 60
        };

        Assert(
            !DocumentIndexer.NeedsReindex(document, "same-hash", "model", 350, 60),
            "unchanged document should be skipped");
        Assert(
            DocumentIndexer.NeedsReindex(document, "changed-hash", "model", 350, 60),
            "changed document hash should trigger indexing");
        Assert(
            DocumentIndexer.NeedsReindex(document, "same-hash", "new-model", 350, 60),
            "changed model should trigger indexing");
    }

    private static async Task TestStoreReplacementAsync(CancellationToken cancellationToken)
    {
        var temporaryDirectory = Path.Combine(
            Path.GetTempPath(),
            $"GamebookAgents-RagSelfTest-{Guid.NewGuid():N}");
        Directory.CreateDirectory(temporaryDirectory);

        try
        {
            var sourcePath = Path.Combine(temporaryDirectory, "reference.txt");
            var store = new RagStore(Path.Combine(temporaryDirectory, "index.json"));
            var firstDocument = CreateDocument(sourcePath, "hash-one", 2);
            await store.ReplaceDocumentAsync(
                firstDocument,
                [
                    CreateChunk(sourcePath, "old-1", 1),
                    CreateChunk(sourcePath, "old-2", 2)
                ],
                cancellationToken);

            var secondDocument = CreateDocument(sourcePath, "hash-two", 1);
            await store.ReplaceDocumentAsync(
                secondDocument,
                [CreateChunk(sourcePath, "new-1", 1)],
                cancellationToken);

            var storedDocument = await store.GetDocumentAsync(sourcePath, cancellationToken);
            var chunks = await store.GetChunksAsync(cancellationToken);
            Assert(storedDocument?.SourceHash == "hash-two", "changed document metadata was not replaced");
            Assert(chunks.Count == 1 && chunks[0].Text == "new-1", "old document chunks were not replaced");
        }
        finally
        {
            if (Directory.Exists(temporaryDirectory))
            {
                Directory.Delete(temporaryDirectory, recursive: true);
            }
        }
    }

    private static IndexedDocument CreateDocument(string sourcePath, string hash, int chunkCount) => new()
    {
        SourcePath = sourcePath,
        SourceFilename = Path.GetFileName(sourcePath),
        SourceHash = hash,
        EmbeddingModel = "test-model",
        EmbeddingDimension = 2,
        ChunkSizeWords = 10,
        ChunkOverlapWords = 2,
        ChunkCount = chunkCount,
        IndexedAtUtc = DateTimeOffset.UtcNow
    };

    private static DocumentChunk CreateChunk(string sourcePath, string text, int chunkNumber) => new()
    {
        Id = $"chunk-{chunkNumber}",
        SourcePath = sourcePath,
        SourceFilename = Path.GetFileName(sourcePath),
        ChunkNumber = chunkNumber,
        Text = text,
        Embedding = [1, 0]
    };

    private static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException($"RAG self-test failed: {message}.");
        }
    }

    private static void AssertAlmostEqual(double expected, double actual, string message)
    {
        Assert(Math.Abs(expected - actual) < 0.000001, $"{message}; expected {expected}, got {actual}");
    }
}
