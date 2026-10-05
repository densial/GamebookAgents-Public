namespace GamebookAgents.Functions;

internal static class ProjectPathGuard
{
    private static readonly string ProjectRoot = FindProjectRoot();
    private static readonly HashSet<string> ProtectedDirectoryNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ".git", ".agents", ".codex", ".idea", ".vs", "bin", "obj"
    };

    internal static string ProjectRootPath => ProjectRoot;

    public static bool TryResolveExistingDirectory(
        string path,
        out string fullPath,
        out string error)
    {
        return TryResolveExistingPath(path, isDirectory: true, out fullPath, out error);
    }

    public static bool TryResolveExistingFile(
        string path,
        out string fullPath,
        out string error)
    {
        return TryResolveExistingPath(path, isDirectory: false, out fullPath, out error);
    }

    public static bool TryResolveCreatableDirectory(
        string path,
        out string fullPath,
        out string error)
    {
        fullPath = string.Empty;
        error = string.Empty;

        if (!TryGetProjectPath(path, out var candidatePath, out error))
        {
            return false;
        }

        if (File.Exists(candidatePath))
        {
            error = $"Error: The path refers to a file, not a directory: {candidatePath}";
            return false;
        }

        var existingAncestor = candidatePath;
        var missingSegments = new Stack<string>();

        try
        {
            while (!Directory.Exists(existingAncestor))
            {
                var entry = new DirectoryInfo(existingAncestor);
                entry.Refresh();

                if (entry.LinkTarget is not null)
                {
                    error = $"Error: Creating a directory through a symbolic link is not allowed: {candidatePath}";
                    return false;
                }

                var segment = Path.GetFileName(existingAncestor);
                var parentPath = Path.GetDirectoryName(existingAncestor);

                if (string.IsNullOrEmpty(segment) || string.IsNullOrEmpty(parentPath))
                {
                    error = $"Error: Could not find an existing parent directory for: {candidatePath}";
                    return false;
                }

                missingSegments.Push(segment);
                existingAncestor = parentPath;
            }

            fullPath = ResolveSymbolicLinks(existingAncestor);

            if (!IsInsideProject(fullPath))
            {
                error = $"Error: Access through a symbolic link outside the project root is not allowed: {candidatePath}";
                fullPath = string.Empty;
                return false;
            }

            while (missingSegments.TryPop(out var segment))
            {
                fullPath = Path.Combine(fullPath, segment);
            }
        }
        catch (Exception exception) when (exception is IOException
                                          or UnauthorizedAccessException)
        {
            error = $"Error: Could not validate path {candidatePath}. {exception.Message}";
            fullPath = string.Empty;
            return false;
        }

        return true;
    }

    public static bool TryResolveWritableFile(
        string path,
        out string fullPath,
        out string error)
    {
        fullPath = string.Empty;
        error = string.Empty;

        if (!TryGetProjectPath(path, out var candidatePath, out error))
        {
            return false;
        }

        if (Directory.Exists(candidatePath))
        {
            error = $"Error: The path refers to a directory, not a file: {candidatePath}";
            return false;
        }

        var parentPath = Path.GetDirectoryName(candidatePath);

        if (string.IsNullOrEmpty(parentPath) || !Directory.Exists(parentPath))
        {
            error = $"Error: The parent directory does not exist: {parentPath}";
            return false;
        }

        try
        {
            var resolvedParentPath = ResolveSymbolicLinks(parentPath);

            if (!IsInsideProject(resolvedParentPath))
            {
                error = $"Error: Access through a symbolic link outside the project root is not allowed: {candidatePath}";
                return false;
            }

            fullPath = Path.Combine(resolvedParentPath, Path.GetFileName(candidatePath));

            // Do not write through a file symlink, even when its current target is inside
            // the project. This avoids a link being used to redirect an overwrite.
            var destination = new FileInfo(fullPath);
            destination.Refresh();

            if (destination.LinkTarget is not null)
            {
                error = $"Error: Writing through a symbolic link is not allowed: {candidatePath}";
                fullPath = string.Empty;
                return false;
            }
        }
        catch (Exception exception) when (exception is IOException
                                          or UnauthorizedAccessException)
        {
            error = $"Error: Could not validate path {candidatePath}. {exception.Message}";
            fullPath = string.Empty;
            return false;
        }

        return true;
    }

    public static bool IsInProtectedProjectDirectory(string path)
    {
        var relativePath = Path.GetRelativePath(ProjectRoot, path);
        var directoryPath = Path.GetDirectoryName(relativePath);

        return directoryPath is not null
               && directoryPath
                   .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                   .Any(ProtectedDirectoryNames.Contains);
    }

    public static bool IsProtectedDirectoryPath(string path)
    {
        var relativePath = Path.GetRelativePath(ProjectRoot, path);

        return relativePath
            .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            .Any(ProtectedDirectoryNames.Contains);
    }

    private static bool TryResolveExistingPath(
        string path,
        bool isDirectory,
        out string fullPath,
        out string error)
    {
        fullPath = string.Empty;
        error = string.Empty;

        if (!TryGetProjectPath(path, out var candidatePath, out error))
        {
            return false;
        }

        var exists = isDirectory
            ? Directory.Exists(candidatePath)
            : File.Exists(candidatePath);

        if (!exists)
        {
            var entryType = isDirectory ? "Directory" : "File";
            error = $"Error: {entryType} does not exist: {candidatePath}";
            return false;
        }

        try
        {
            fullPath = ResolveSymbolicLinks(candidatePath);
        }
        catch (Exception exception) when (exception is IOException
                                          or UnauthorizedAccessException)
        {
            error = $"Error: Could not validate path {candidatePath}. {exception.Message}";
            return false;
        }

        if (!IsInsideProject(fullPath))
        {
            error = $"Error: Access through a symbolic link outside the project root is not allowed: {candidatePath}";
            fullPath = string.Empty;
            return false;
        }

        return true;
    }

    private static bool TryGetProjectPath(
        string path,
        out string fullPath,
        out string error)
    {
        fullPath = string.Empty;
        error = string.Empty;

        try
        {
            fullPath = Path.GetFullPath(path, ProjectRoot);
        }
        catch (Exception exception) when (exception is ArgumentException
                                          or NotSupportedException
                                          or PathTooLongException)
        {
            error = $"Error: The path is invalid. {exception.Message}";
            return false;
        }

        if (!IsInsideProject(fullPath))
        {
            error = $"Error: Access outside the project root is not allowed: {fullPath}";
            fullPath = string.Empty;
            return false;
        }

        return true;
    }

    private static string ResolveSymbolicLinks(string path)
    {
        var relativePath = Path.GetRelativePath(ProjectRoot, path);
        var resolvedPath = ProjectRoot;

        if (relativePath == ".")
        {
            return resolvedPath;
        }

        foreach (var segment in relativePath.Split(Path.DirectorySeparatorChar))
        {
            var nextPath = Path.Combine(resolvedPath, segment);
            FileSystemInfo entry = Directory.Exists(nextPath)
                ? new DirectoryInfo(nextPath)
                : new FileInfo(nextPath);

            var linkTarget = entry.ResolveLinkTarget(returnFinalTarget: true);
            resolvedPath = Path.GetFullPath(linkTarget?.FullName ?? nextPath);

            if (!IsInsideProject(resolvedPath))
            {
                return resolvedPath;
            }
        }

        return resolvedPath;
    }

    private static bool IsInsideProject(string path)
    {
        var relativePath = Path.GetRelativePath(ProjectRoot, path);

        return relativePath == "."
               || (!Path.IsPathRooted(relativePath)
                   && relativePath != ".."
                   && !relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal));
    }

    private static string FindProjectRoot()
    {
        foreach (var startPath in new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() })
        {
            for (var directory = new DirectoryInfo(startPath); directory is not null; directory = directory.Parent)
            {
                if (directory.EnumerateFiles("*.csproj", SearchOption.TopDirectoryOnly).Any()
                    || directory.EnumerateFiles("*.sln", SearchOption.TopDirectoryOnly).Any()
                    || directory.EnumerateFiles("*.slnx", SearchOption.TopDirectoryOnly).Any())
                {
                    return directory.FullName;
                }
            }
        }

        throw new InvalidOperationException("Could not locate the project or solution root.");
    }
}
