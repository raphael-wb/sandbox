using System.Management.Automation;
using Microsoft.PowerShell.Commands;

namespace Rwb.PowerShell
{
	// ReSharper disable once InconsistentNaming
	public static class PSCmdletExtensions
	{
		private static IPathResolver PathResolver => ServiceLocator.Get<IPathResolver>();

		// ReSharper disable InconsistentNaming
		public static string GetProviderPathFromPSPath(this PSCmdlet cmdlet, string psPath)
		{
			return PathResolver.GetProviderPathFromPSPath(cmdlet.SessionState.Path, psPath);
		}

		public static string GetFileSystemProviderPathFromPSPath(this PSCmdlet cmdlet, string psPath)
		{
			return PathResolver.GetProviderPathFromPSPath<FileSystemProvider>(cmdlet.SessionState.Path, psPath);
		}
		// ReSharper restore InconsistentNaming

		public static string ResolveFileSystemDestinationPathFromPsPath(this PSCmdlet cmdlet, string destinationPsPath, string inputPath)
		{
			return PathResolver.ResolveFileSystemDestinationPathFromPSPath(cmdlet.SessionState.Path, destinationPsPath, inputPath);
		}
	}
}
