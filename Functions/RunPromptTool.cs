using System.ComponentModel;
using System.Text;
using GamebookAgents.Configuration;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

namespace GamebookAgents.Functions;

/// <summary>
/// Runs one production prompt in an isolated Glimmer session. The child gets
/// the same artifact writer so the application's one-write-per-artifact rule
/// remains authoritative.
/// </summary>
public sealed class RunPromptTool
{
    private const int MaximumInputFiles = 30;
    private const int MaximumReturnedCharacters = 4_000;

    private readonly ChatClient _chatClient;
    private readonly ArtifactWriteTool _artifactWriteTool;
    private readonly RagTools _ragTools;
    private readonly string _projectRoot;
    private readonly RagOptions _ragOptions;

    public RunPromptTool(
        ChatClient chatClient,
        ArtifactWriteTool artifactWriteTool,
        RagTools ragTools,
        string projectRoot,
        RagOptions ragOptions)
    {
        _chatClient = chatClient;
        _artifactWriteTool = artifactWriteTool;
        _ragTools = ragTools;
        _projectRoot = Path.GetFullPath(projectRoot);
        _ragOptions = ragOptions;
    }

    [Description("Execute a production prompt file in a fresh Glimmer session for the current artifact. Supply the known small project input files whose full contents the prompt requires. Large configured references are retrieved by the child through SearchReferenceMaterial instead of being inserted into context.")]
    public async Task<string> RunPrompt(
        [Description("Path to a production prompt under builder/prompts.")]
        string promptPath,
        [Description("Paths to known small authoritative input files that must be included in full. Do not include a large indexed reference document.")]
        string[]? inputFiles = null,
        [Description("Optional concise artifact-specific context not already present in the input files.")]
        string? context = null)
    {
        Console.WriteLine($"RunPrompt:{promptPath}");

        if (!TryResolvePrompt(promptPath, out var fullPromptPath, out var error))
        {
            return error;
        }

        inputFiles ??= [];
        if (inputFiles.Length > MaximumInputFiles)
        {
            return $"Error: RunPrompt accepts at most {MaximumInputFiles} input files per artifact.";
        }

        try
        {
            var childPrompt = await BuildChildPromptAsync(
                fullPromptPath,
                inputFiles,
                context);

            AIAgent childAgent = _chatClient.AsAIAgent(
                name: "GamebookProductionPrompt",
                instructions: """
                    You execute one supplied gamebook production prompt and produce at most one artifact.
                    The supplied production prompt and project files are authoritative.

                    Use SearchReferenceMaterial for focused questions about configured large references such as the SRD, rules, lore, and setting sourcebooks. Do not read an entire large reference into context. Use ReadFile for known small project files whose complete contents are required, including production prompts, BOOK_BRIEF.md, OUTPUT_SPEC.md, VALIDATION_RULES.md, chapter plans, and sequence plans.

                    Inspect additional project files only when the production prompt requires them. Write the completed artifact with WriteFile. Once WriteFile succeeds, stop using tools and return a concise summary. Never write a second artifact. If blocked, explain the exact blocker without writing an unrelated file.
                    """,
                tools:
                [
                    AIFunctionFactory.Create(DirectoryFunctions.CreateDirectory, name: "CreateDirectory"),
                    AIFunctionFactory.Create(DirectoryFunctions.ListDirectory, name: "ListDirectory"),
                    AIFunctionFactory.Create(FileFunctions.ReadFile, name: "ReadFile"),
                    AIFunctionFactory.Create(FileFunctions.ReadPdf, name: "ReadPdf"),
                    AIFunctionFactory.Create(_ragTools.SearchReferenceMaterial, name: "SearchReferenceMaterial"),
                    AIFunctionFactory.Create(_artifactWriteTool.WriteFile, name: "WriteFile"),
                    AIFunctionFactory.Create(StatusFunctions.ReportStatus, name: "ReportStatus")
                ]);

            var session = await childAgent.CreateSessionAsync();
            var response = await childAgent.RunAsync(childPrompt, session);
            var responseText = response.Text ?? string.Empty;

            if (responseText.Length > MaximumReturnedCharacters)
            {
                responseText = $"{responseText[..MaximumReturnedCharacters]}\n[Child response truncated]";
            }

            return _artifactWriteTool.WrittenPath is null
                ? $"RunPrompt completed without writing an artifact. Child response: {responseText}"
                : $"RunPrompt completed. Artifact written: {_artifactWriteTool.WrittenPath}. Child response: {responseText}";
        }
        catch (Exception exception) when (exception is IOException
                                          or UnauthorizedAccessException
                                          or InvalidOperationException)
        {
            return $"Error: RunPrompt failed. {exception.Message}";
        }
    }

    private async Task<string> BuildChildPromptAsync(
        string fullPromptPath,
        IReadOnlyList<string> inputFiles,
        string? context)
    {
        var output = new StringBuilder();
        output.AppendLine("# PRODUCTION PROMPT");
        output.AppendLine();
        output.AppendLine(await File.ReadAllTextAsync(fullPromptPath));

        if (!string.IsNullOrWhiteSpace(context))
        {
            output.AppendLine();
            output.AppendLine("# ARTIFACT CONTEXT");
            output.AppendLine();
            output.AppendLine(context.Trim());
        }

        output.AppendLine();
        output.AppendLine("# REQUIRED INPUT FILES");

        foreach (var inputPath in inputFiles)
        {
            if (!ProjectPathGuard.TryResolveExistingFile(inputPath, out var fullPath, out var error))
            {
                throw new InvalidOperationException(error);
            }

            output.AppendLine();
            output.AppendLine($"## {Path.GetRelativePath(_projectRoot, fullPath)}");
            output.AppendLine();

            if (_ragOptions.IsConfiguredReferenceDocument(_projectRoot, fullPath))
            {
                output.AppendLine(
                    "[This large reference is indexed. Use SearchReferenceMaterial with focused queries; "
                    + "its full contents were intentionally not added to this session.]");
                continue;
            }

            output.AppendLine(await File.ReadAllTextAsync(fullPath));
        }

        return output.ToString();
    }

    private bool TryResolvePrompt(string path, out string fullPath, out string error)
    {
        if (!ProjectPathGuard.TryResolveExistingFile(path, out fullPath, out error))
        {
            return false;
        }

        var promptRoot = Path.Combine(_projectRoot, "builder", "prompts");
        var relativePath = Path.GetRelativePath(promptRoot, fullPath);
        if (Path.IsPathRooted(relativePath)
            || relativePath == ".."
            || relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
        {
            error = $"Error: Production prompts must be under {promptRoot}.";
            fullPath = string.Empty;
            return false;
        }

        return true;
    }
}
