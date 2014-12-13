using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Rwb
{
	public interface IFileSystem
	{
		bool DirectoryExists(string path);
		bool FileExists(string path);
		void CreateDirectory(string path);
		Stream OpenRead(string path);
		Stream OpenWrite(string path);
		Stream Open(string path, FileMode mode, FileAccess access, FileShare share = FileShare.None, FileOptions fileOptions = FileOptions.None);
		IEnumerable<string> EnumerateFiles(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);
		IEnumerable<string> EnumerateDirectories(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);
		void Copy(string sourceFileName, string destinationFileName, bool overwrite = false);
		Task CopyAsync(string sourceFileName, string destinationFileName, bool overwrite = false);
		void Move(string sourceFileName, string destinationFileName);
		void DeleteFile(string path);
		void SetAttributes(string path, FileAttributes fileAttributes);
		void Encrypt(string path);

		DateTime GetLastWriteTimeUtc(string path);
		void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc);
	}
}
