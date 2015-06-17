using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Provider;
using Microsoft.PowerShell.Commands;

namespace Rwb.PowerShell
{
	public class PathResolver : IPathResolver
	{
		private static IFileSystem FileSystem => ServiceLocator.Get<IFileSystem>();

		public string GetProviderPathFromPSPath<TExpectedProvider>(PathIntrinsics pathIntrinsics, string psPath)
			where TExpectedProvider : CmdletProvider
		{
			return GetProviderPathFromPSPath(pathIntrinsics, psPath, typeof(TExpectedProvider));
		}

		public string GetProviderPathFromPSPath(PathIntrinsics pathIntrinsics, string psPath)
		{
			return GetProviderPathFromPSPath(pathIntrinsics, psPath, null);
		}

		private static string GetProviderPathFromPSPath(PathIntrinsics pathIntrinsics, string psPath, Type expectedProviderType)
		{
			ProviderInfo providerInfo;
			PSDriveInfo driveInfo;
			string providerPath = pathIntrinsics.GetUnresolvedProviderPathFromPSPath(
				psPath, out providerInfo, out driveInfo);

			if ((expectedProviderType != null) &&
				(providerInfo.ImplementingType != expectedProviderType))
			{
				throw new ArgumentException(
					string.Format(Resources.InvalidProviderError, psPath, expectedProviderType));
			}

			return providerPath;
		}

		public IEnumerable<string> GetResolvedProviderPathsFromPSPath<TExpectedProvider>(PathIntrinsics pathIntrinsics, string psPath)
			where TExpectedProvider : CmdletProvider
		{
			return GetResolvedProviderPathsFromPSPath(pathIntrinsics, psPath, typeof(TExpectedProvider));
		}


		public IEnumerable<string> GetResolvedProviderPathsFromPSPath(PathIntrinsics pathIntrinsics, string psPath)
		{
			return GetResolvedProviderPathsFromPSPath(pathIntrinsics, psPath, null);
		}

		private IEnumerable<string> GetResolvedProviderPathsFromPSPath(PathIntrinsics pathIntrinsics, string psPath, Type expectedProviderType)
		{
			ProviderInfo providerInfo;
			IEnumerable<string> resolvedPaths = pathIntrinsics.GetResolvedProviderPathFromPSPath(
				psPath, out providerInfo);

			if ((expectedProviderType != null) &&
				(providerInfo.ImplementingType != expectedProviderType))
			{
				throw new ArgumentException(
					string.Format(Resources.InvalidProviderError, psPath, expectedProviderType));
			}

			return resolvedPaths;
		}

		public string ResolveFileSystemDestinationPathFromPSPath(PathIntrinsics pathIntrinsics, string destinationPSPath, string inputPath)
		{
			if (string.IsNullOrEmpty(destinationPSPath))
				destinationPSPath = ".";
			string destinationPath = GetProviderPathFromPSPath<FileSystemProvider>(
				pathIntrinsics, destinationPSPath);

			return FileSystem.DirectoryExists(destinationPath)
				? Path.Combine(destinationPath, Path.GetFileName(inputPath))
				: destinationPath;
		}
	}
}
