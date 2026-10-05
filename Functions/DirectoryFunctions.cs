using System.ComponentModel;

namespace GamebookAgents.Functions;

public static class DirectoryFunctions
{
    [Description("Create a directory inside the project.")]
    public static string CreateDirectory(
        [Description("The path of the directory to create.")] string path)
    {
        
        Console.WriteLine($"CreateDirectory:{path}");
        if (string.IsNullOrWhiteSpace(path))
        {
            return "Error: A directory path is required.";
        }

        if (!ProjectPathGuard.TryResolveCreatableDirectory(path, out var fullPath, out var error))
        {
            return error;
        }

        if (ProjectPathGuard.IsProtectedDirectoryPath(fullPath))
        {
            return $"Error: Protected project directories cannot be created or modified: {fullPath}";
        }

        try
        {
            var alreadyExists = Directory.Exists(fullPath);
            Directory.CreateDirectory(fullPath);

            return alreadyExists
                ? $"Directory already exists: {fullPath}"
                : $"Directory created successfully: {fullPath}";
        }
        catch (UnauthorizedAccessException exception)
        {
            return $"Error: Access denied for directory {fullPath}. {exception.Message}";
        }
        catch (IOException exception)
        {
            return $"Error: Could not create directory {fullPath}. {exception.Message}";
        }
    }

    [Description("List all files and folders directly inside a directory.")]
    public static string ListDirectory(
        [Description("The path of the directory to list.")] string path)
    {
        Console.WriteLine($"ListDirectory:{path}");
        if (string.IsNullOrWhiteSpace(path))
        {
            return "Error: A directory path is required.";
        }

        if (!ProjectPathGuard.TryResolveExistingDirectory(path, out var fullPath, out var error))
        {
            return error;
        }

        try
        {
            var entries = Directory
                .EnumerateFileSystemEntries(fullPath)
                .Select(entry => new
                {
                    Name = Path.GetFileName(entry),
                    IsDirectory = Directory.Exists(entry)
                })
                .OrderByDescending(entry => entry.IsDirectory)
                .ThenBy(entry => entry.Name, StringComparer.OrdinalIgnoreCase)
                .Select(entry => entry.IsDirectory
                    ? $"[Directory] {entry.Name}"
                    : $"[File] {entry.Name}")
                .ToArray();

            return entries.Length == 0
                ? $"Directory is empty: {fullPath}"
                : $"Contents of {fullPath}:{Environment.NewLine}{string.Join(Environment.NewLine, entries)}";
        }
        catch (UnauthorizedAccessException exception)
        {
            return $"Error: Access denied for directory {fullPath}. {exception.Message}";
        }
        catch (IOException exception)
        {
            return $"Error: Could not list directory {fullPath}. {exception.Message}";
        }
    }
}
