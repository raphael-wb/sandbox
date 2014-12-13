using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Rwb
{
	public class FileSystem : IFileSystem
	{
		public static StringComparer PathComparer => StringComparer.OrdinalIgnoreCase;

		public bool FileExists(string path) => File.Exists(path);

		public bool DirectoryExists(string path) => Directory.Exists(path);

		public void CreateDirectory(string path) => Directory.CreateDirectory(path);

		public Stream OpenRead(string path) => File.OpenRead(path);

		public Stream OpenWrite(string path) => File.OpenWrite(path);

		public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
			Directory.EnumerateFiles(path, searchPattern, searchOption);

		public IEnumerable<string> EnumerateDirectories(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly) =>
			Directory.EnumerateDirectories(path, searchPattern, searchOption);

		public void Copy(string sourceFileName, string destinationFileName, bool overwrite) =>
			File.Copy(sourceFileName, destinationFileName, overwrite);

		public async Task CopyAsync(string sourceFileName, string destinationFileName, bool overwrite)
		{
			Require.NotEmpty(sourceFileName, nameof(sourceFileName));
			Require.NotEmpty(destinationFileName, nameof(destinationFileName));

			var fileMode = overwrite ? FileMode.Create : FileMode.CreateNew;

			using (var sourceStream = OpenRead(sourceFileName))
			using (var destinationStream = File.Open(destinationFileName, fileMode, FileAccess.Write))
				await sourceStream.CopyToAsync(destinationStream);
		}

		public void Move(string sourceFileName, string destinationFileName) => File.Move(sourceFileName, destinationFileName);

		public void DeleteFile(string path) => File.Delete(path);

		public void SetAttributes(string path, FileAttributes fileAttributes) => File.SetAttributes(path, fileAttributes);

		public void Encrypt(string path) => File.Encrypt(path);

		public Stream Open(string path, FileMode mode, FileAccess access, FileShare share, FileOptions fileOptions) =>
			new FileStream(path, mode, access, share, 4096, fileOptions);

		public DateTime GetLastWriteTimeUtc(string path) => File.GetLastWriteTimeUtc(path);

		public void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
		{
			Require.Utc(lastWriteTimeUtc, nameof(lastWriteTimeUtc));
			File.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
		}
	}
}
