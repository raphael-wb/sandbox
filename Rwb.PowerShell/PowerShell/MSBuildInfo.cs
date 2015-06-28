using System;

namespace Rwb.PowerShell
{
	public class MSBuildInfo
	{
		public Version Version { get; }
		public string Path { get; }

		public MSBuildInfo(Version version, string path)
		{
			Version = Require.NotNull(version, nameof(version));
			Path = Require.AbsolutePath(path, nameof(path));
		}
	}
}