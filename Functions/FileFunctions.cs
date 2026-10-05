using System.ComponentModel;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace GamebookAgents.Functions;

public static class FileFunctions
{
    private const int MaximumPdfPagesPerRead = 10;
    private const int MaximumPdfCharactersPerRead = 32_000;

    private static readonly HashSet<string> ProtectedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".cs", ".csproj", ".fs", ".fsproj", ".vb", ".vbproj",
        ".sln", ".slnx", ".props", ".targets", ".dll", ".exe", ".pdb",
        ".sh", ".bash", ".zsh", ".ps1", ".bat", ".cmd"
    };

    [Description("Read and return the contents of a file.")]
    public static string ReadFile(
        [Description("The path of the file to read.")] string path)
    {
        Console.WriteLine($"ReadFile:{path}");
        if (string.IsNullOrWhiteSpace(path))
        {
            return "Error: A file path is required.";
        }

        if (!ProjectPathGuard.TryResolveExistingFile(path, out var fullPath, out var error))
        {
            return error;
        }

        try
        {
            return File.ReadAllText(fullPath);
        }
        catch (UnauthorizedAccessException exception)
        {
            return $"Error: Access denied for file {fullPath}. {exception.Message}";
        }
        catch (IOException exception)
        {
            return $"Error: Could not read file {fullPath}. {exception.Message}";
        }
    }

    [Description("Extract text from at most 10 PDF pages per call. If pages are omitted, reads the first 10 pages. Request later pages in subsequent calls.")]
    public static string ReadPdf(
        [Description("The path of the PDF file to read.")] string path,
        [Description("Optional list of up to 10 one-based page numbers. Omit or pass an empty list to read the first 10 pages.")]
        int[]? pages = null)
    {
        Console.WriteLine($"ReadPdf:{path}");
        if (string.IsNullOrWhiteSpace(path))
        {
            return "Error: A PDF file path is required.";
        }

        if (!ProjectPathGuard.TryResolveExistingFile(path, out var fullPath, out var error))
        {
            return error;
        }

        if (IsProtectedProgramFile(fullPath))
        {
            return $"Error: Program files cannot be read as PDFs: {fullPath}";
        }

        if (!Path.GetExtension(fullPath).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return $"Error: The file must have a .pdf extension: {fullPath}";
        }

        try
        {
            using var document = PdfDocument.Open(fullPath);

            var selectedPages = pages is null || pages.Length == 0
                ? Enumerable.Range(1, Math.Min(MaximumPdfPagesPerRead, document.NumberOfPages)).ToArray()
                : pages.Distinct().ToArray();

            if (selectedPages.Length > MaximumPdfPagesPerRead)
            {
                return $"Error: ReadPdf can extract at most {MaximumPdfPagesPerRead} pages per call. Split the request into smaller page ranges.";
            }

            var invalidPages = selectedPages
                .Where(page => page < 1 || page > document.NumberOfPages)
                .ToArray();

            if (invalidPages.Length > 0)
            {
                return $"Error: Page numbers must be between 1 and {document.NumberOfPages}. "
                       + $"Invalid pages: {string.Join(", ", invalidPages)}";
            }

            var result = new StringBuilder();
            var outputTruncated = false;

            foreach (var pageNumber in selectedPages)
            {
                if (result.Length > 0)
                {
                    result.AppendLine();
                    result.AppendLine();
                }

                var page = document.GetPage(pageNumber);
                var text = ContentOrderTextExtractor.GetText(page).Trim();

                result.AppendLine($"--- Page {pageNumber} ---");
                var pageText = string.IsNullOrEmpty(text)
                    ? "[No extractable text on this page]"
                    : text;
                var remainingCharacters = MaximumPdfCharactersPerRead - result.Length;
                if (pageText.Length > remainingCharacters)
                {
                    result.Append(pageText.AsSpan(0, Math.Max(0, remainingCharacters)));
                    result.AppendLine();
                    result.Append("[Output truncated at the ");
                    result.Append(MaximumPdfCharactersPerRead);
                    result.AppendLine(" character limit. Request a smaller page range.] ");
                    outputTruncated = true;
                    break;
                }

                result.Append(pageText);
            }

            if (result.Length == 0)
            {
                return "The PDF contains no pages.";
            }

            if (!outputTruncated && (pages is null || pages.Length == 0) && selectedPages[^1] < document.NumberOfPages)
            {
                result.AppendLine();
                result.Append($"[Showing pages {selectedPages[0]}-{selectedPages[^1]} of {document.NumberOfPages}. Request subsequent pages in batches of up to {MaximumPdfPagesPerRead}.]");
            }

            return result.ToString();
        }
        catch (UnauthorizedAccessException exception)
        {
            return $"Error: Access denied for PDF {fullPath}. {exception.Message}";
        }
        catch (IOException exception)
        {
            return $"Error: Could not read PDF {fullPath}. {exception.Message}";
        }
        catch (Exception exception)
        {
            return $"Error: Could not extract text from PDF {fullPath}. {exception.Message}";
        }
    }

    [Description("Create or overwrite a text file inside the project.")]
    public static string WriteFile(
        [Description("The path of the file to write, including its file name.")] string path,
        [Description("The complete text to write to the file.")] string contents)
    {
        Console.WriteLine($"WriteFile:{path}");
        if (string.IsNullOrWhiteSpace(path))
        {
            return "Error: A file path is required.";
        }

        if (!ProjectPathGuard.TryResolveWritableFile(path, out var fullPath, out var error))
        {
            return error;
        }

        if (IsProtectedProgramFile(fullPath))
        {
            return $"Error: Program files cannot be created or modified: {fullPath}";
        }

        try
        {
            File.WriteAllText(fullPath, contents);
            return $"File written successfully: {fullPath}";
        }
        catch (UnauthorizedAccessException exception)
        {
            return $"Error: Access denied for file {fullPath}. {exception.Message}";
        }
        catch (IOException exception)
        {
            return $"Error: Could not write file {fullPath}. {exception.Message}";
        }
    }

    private static bool IsProtectedProgramFile(string path)
    {
        var fileName = Path.GetFileName(path);

        return ProjectPathGuard.IsInProtectedProjectDirectory(path)
               || ProtectedExtensions.Contains(Path.GetExtension(path))
               || fileName.EndsWith(".deps.json", StringComparison.OrdinalIgnoreCase)
               || fileName.EndsWith(".runtimeconfig.json", StringComparison.OrdinalIgnoreCase)
               || (fileName.StartsWith("appsettings", StringComparison.OrdinalIgnoreCase)
                   && fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
               || fileName.Equals("global.json", StringComparison.OrdinalIgnoreCase)
               || fileName.Equals("NuGet.Config", StringComparison.OrdinalIgnoreCase)
               || fileName.Equals("launchSettings.json", StringComparison.OrdinalIgnoreCase)
               || fileName.Equals(".editorconfig", StringComparison.OrdinalIgnoreCase)
               || fileName.Equals(".gitignore", StringComparison.OrdinalIgnoreCase)
               || fileName.Equals(".gitattributes", StringComparison.OrdinalIgnoreCase)
               || fileName.Equals("Directory.Build.props", StringComparison.OrdinalIgnoreCase)
               || fileName.Equals("Directory.Build.targets", StringComparison.OrdinalIgnoreCase)
               || fileName.Equals("Directory.Packages.props", StringComparison.OrdinalIgnoreCase);
    }
}
