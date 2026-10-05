using System.ComponentModel;

namespace GamebookAgents.Functions;

/// <summary>
/// Limits an agent run to one successfully written artifact so the caller can
/// start the next artifact with a fresh session.
/// </summary>
public sealed class ArtifactWriteTool
{
    private readonly object _gate = new();

    public string? WrittenPath { get; private set; }

    public void BeginRun()
    {
        lock (_gate)
        {
            WrittenPath = null;
        }
    }

    [Description("Create or overwrite one text artifact inside the project. Only one successful write is allowed per agent run.")]
    public string WriteFile(
        [Description("The path of the file to write, including its file name.")] string path,
        [Description("The complete text to write to the file.")] string contents)
    {
        lock (_gate)
        {
            if (WrittenPath is not null)
            {
                return $"Error: This run already wrote the artifact {WrittenPath}. "
                       + "End the run now; the next artifact will use a fresh session.";
            }

            var result = FileFunctions.WriteFile(path, contents);

            if (result.StartsWith("File written successfully:", StringComparison.Ordinal))
            {
                WrittenPath = path;
            }

            return result;
        }
    }
}
