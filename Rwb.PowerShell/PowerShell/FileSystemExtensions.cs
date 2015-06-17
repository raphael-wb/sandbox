using System.Management.Automation;

namespace Rwb.PowerShell
{
    public static class FileSystemExtensions
    {
        public static void AssertFileExists(this IFileSystem fileSystem, string path)
        {
            if (!fileSystem.FileExists(path))
                throw new ItemNotFoundException(string.Format(Resources.FileNotFoundError, path));
        }

        public static void AssertDirectoryExists(this IFileSystem fileSystem, string path)
        {
            if (!fileSystem.DirectoryExists(path))
                throw new ItemNotFoundException(string.Format(Resources.DirectoryNotFoundError, path));
        }

        public static void AssertFileSystemItemExists(this IFileSystem fileSystem, string path)
        {
            if (!(fileSystem.FileExists(path) || fileSystem.DirectoryExists(path)))
                throw new ItemNotFoundException(string.Format(Resources.FileSystemItemNotFoundError, path));
        }
    }
}
