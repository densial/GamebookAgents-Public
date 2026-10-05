using System.Text.Json;

namespace GamebookAgents.Configuration;

public sealed class GamebookOptions
{
    public string LmStudioBaseUrl { get; set; } = "http://127.0.0.1:1234/v1/";

    public string GenerationModel { get; set; } = "unsloth/muse-glimmer-30b";

    public string EmbeddingModel { get; set; } = "text-embedding-nomic-embed-text-v1.5";

    public int EmbeddingRequestTimeoutSeconds { get; set; } = 120;

    public RagOptions Rag { get; set; } = new();

    public static async Task<GamebookOptions> LoadAsync(
        string projectRoot,
        CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(projectRoot, "gamebooksettings.json");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"Application configuration was not found: {path}",
                path);
        }

        try
        {
            await using var stream = File.OpenRead(path);
            var options = await JsonSerializer.DeserializeAsync<GamebookOptions>(
                              stream,
                              new JsonSerializerOptions
                              {
                                  PropertyNameCaseInsensitive = true
                              },
                              cancellationToken)
                          ?? throw new InvalidDataException($"Configuration file is empty: {path}");

            options.ApplyEnvironmentOverrides();
            options.Validate(projectRoot);
            return options;
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException(
                $"Configuration file contains invalid JSON: {path}. {exception.Message}",
                exception);
        }
    }

    public Uri GetLmStudioBaseUri()
    {
        var value = LmStudioBaseUrl.EndsWith("/", StringComparison.Ordinal)
            ? LmStudioBaseUrl
            : $"{LmStudioBaseUrl}/";

        return new Uri(value, UriKind.Absolute);
    }

    private void ApplyEnvironmentOverrides()
    {
        LmStudioBaseUrl = Environment.GetEnvironmentVariable("GAMEBOOK_LM_STUDIO_URL")
                          ?? LmStudioBaseUrl;
        GenerationModel = Environment.GetEnvironmentVariable("GAMEBOOK_GENERATION_MODEL")
                          ?? GenerationModel;
        EmbeddingModel = Environment.GetEnvironmentVariable("GAMEBOOK_EMBEDDING_MODEL")
                         ?? EmbeddingModel;
    }

    private void Validate(string projectRoot)
    {
        if (!Uri.TryCreate(LmStudioBaseUrl, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidDataException($"LmStudioBaseUrl is not a valid HTTP URL: {LmStudioBaseUrl}");
        }

        if (string.IsNullOrWhiteSpace(GenerationModel))
        {
            throw new InvalidDataException("GenerationModel is required.");
        }

        if (string.IsNullOrWhiteSpace(EmbeddingModel))
        {
            throw new InvalidDataException("EmbeddingModel is required.");
        }

        if (EmbeddingRequestTimeoutSeconds <= 0)
        {
            throw new InvalidDataException("EmbeddingRequestTimeoutSeconds must be greater than zero.");
        }

        Rag.Validate(projectRoot);
    }
}

public sealed class RagOptions
{
    public string IndexPath { get; set; } = ".rag/reference-index.json";

    public int ChunkSizeWords { get; set; } = 350;

    public int ChunkOverlapWords { get; set; } = 60;

    public int EmbeddingBatchSize { get; set; } = 24;

    public int DefaultTopK { get; set; } = 6;

    public List<string> ReferenceDocuments { get; set; } = [];

    public string ResolveIndexPath(string projectRoot) => ResolveProjectPath(projectRoot, IndexPath);

    public IReadOnlyList<string> ResolveReferenceDocuments(string projectRoot) =>
        ReferenceDocuments.Select(path => ResolveProjectPath(projectRoot, path)).ToArray();

    public bool IsConfiguredReferenceDocument(string projectRoot, string path)
    {
        var candidate = Path.GetFullPath(path, projectRoot);

        return ResolveReferenceDocuments(projectRoot)
            .Any(reference => string.Equals(reference, candidate, StringComparison.OrdinalIgnoreCase));
    }

    internal void Validate(string projectRoot)
    {
        if (ChunkSizeWords < 50)
        {
            throw new InvalidDataException("Rag.ChunkSizeWords must be at least 50.");
        }

        if (ChunkOverlapWords < 0 || ChunkOverlapWords >= ChunkSizeWords)
        {
            throw new InvalidDataException(
                "Rag.ChunkOverlapWords must be non-negative and smaller than Rag.ChunkSizeWords.");
        }

        if (EmbeddingBatchSize <= 0)
        {
            throw new InvalidDataException("Rag.EmbeddingBatchSize must be greater than zero.");
        }

        if (DefaultTopK <= 0 || DefaultTopK > 20)
        {
            throw new InvalidDataException("Rag.DefaultTopK must be between 1 and 20.");
        }

        _ = ResolveIndexPath(projectRoot);

        foreach (var document in ReferenceDocuments)
        {
            _ = ResolveProjectPath(projectRoot, document);
        }
    }

    private static string ResolveProjectPath(string projectRoot, string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new InvalidDataException("Configured project paths cannot be empty.");
        }

        var fullPath = Path.GetFullPath(path, projectRoot);
        var relativePath = Path.GetRelativePath(projectRoot, fullPath);

        if (Path.IsPathRooted(relativePath)
            || relativePath == ".."
            || relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Configured path is outside the project root: {path}");
        }

        return fullPath;
    }
}
