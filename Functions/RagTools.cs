using System.ComponentModel;
using System.Text;
using GamebookAgents.RAG;

namespace GamebookAgents.Functions;

public sealed class RagTools
{
    private readonly RagSearchService _searchService;
    private readonly int _defaultTopK;

    public RagTools(RagSearchService searchService, int defaultTopK)
    {
        _searchService = searchService;
        _defaultTopK = defaultTopK;
    }

    [Description("Search large authoritative reference documents such as rules, SRD material, lore, setting information, and other indexed reference sources. Use a focused natural-language query. Use ReadFile instead when a known small project file must be read in full.")]
    public async Task<string> SearchReferenceMaterial(
        [Description("A focused natural-language query describing the specific reference information needed.")]
        string query,
        [Description("Optional number of excerpts to return, from 1 to 20. Omit to use the configured default.")]
        int? topK = null)
    {
        Console.WriteLine($"SearchReferenceMaterial:{query}");

        try
        {
            var results = await _searchService.SearchAsync(query, topK ?? _defaultTopK);
            return FormatResults(results);
        }
        catch (Exception exception) when (exception is RagException
                                          or ArgumentException
                                          or InvalidOperationException)
        {
            return $"Error: Reference search failed. {exception.Message}";
        }
    }

    public static string FormatResults(IReadOnlyList<RagSearchResult> results)
    {
        if (results.Count == 0)
        {
            return "No relevant reference excerpts were found.";
        }

        var output = new StringBuilder();
        output.AppendLine($"Reference search returned {results.Count} excerpt(s). Scores rank relative relevance; verify the cited text before applying it.");

        for (var index = 0; index < results.Count; index++)
        {
            var result = results[index];
            output.AppendLine();
            output.Append($"[{index + 1}] {result.SourceFilename}");

            if (result.PageNumber is not null)
            {
                output.Append($", page {result.PageNumber}");
            }

            output.Append($", chunk {result.ChunkNumber}, score {result.SimilarityScore:F4}");

            if (!string.IsNullOrWhiteSpace(result.Section))
            {
                output.Append($", section: {result.Section}");
            }

            output.AppendLine();
            output.AppendLine($"Source: {result.SourcePath}");
            output.AppendLine(result.Text);
        }

        return output.ToString().TrimEnd();
    }
}
