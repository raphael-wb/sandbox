using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Provider;

namespace Rwb.PowerShell
{
	public interface IPathResolver
	{
		// ReSharper disable InconsistentNaming
		string GetProviderPathFromPSPath(PathIntrinsics pathIntrinsics, string psPath);
		string GetProviderPathFromPSPath<TExpectedProvider>(PathIntrinsics pathIntrinsics, string psPath)
			where TExpectedProvider : CmdletProvider;

		IEnumerable<string> GetResolvedProviderPathsFromPSPath(PathIntrinsics pathIntrinsics, string psPath);
		IEnumerable<string> GetResolvedProviderPathsFromPSPath<TExpectedProvider>(PathIntrinsics pathIntrinsics, string psPath)
			where TExpectedProvider : CmdletProvider;

		string ResolveFileSystemDestinationPathFromPSPath(PathIntrinsics pathIntrinsics, string destinationPSPath, string inputPath);
		// ReSharper restore InconsistentNaming
	}
}
