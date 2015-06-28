using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace Rwb.PowerShell
{
	internal class MSBuildLocator
	{
		private readonly IFileSystem _fileSystem;

		public MSBuildLocator(IFileSystem fileSystem)
		{
			_fileSystem = Require.NotNull(fileSystem, nameof(fileSystem));
		}

		public MSBuildInfo GetLatest()
		{
			return GetAll64().LastOrDefault();
		}

		public IReadOnlyCollection<MSBuildInfo> GetAll64()
		{
			using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			using (var versionListKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\MSBuild\ToolsVersions"))
			{
				return EnumerateVersions(versionListKey).OrderBy(b => b.Version).ToList();
			}
		}

		public MSBuildInfo GetVersion(Version version)
		{
			return GetAll64().SingleOrDefault(b => b.Version == version);
		}

		private IEnumerable<MSBuildInfo> EnumerateVersions(RegistryKey versionListKey)
		{
			foreach (var versionName in versionListKey.GetSubKeyNames())
			{
				using (var versionKey = versionListKey.OpenSubKey(versionName))
				{
					var msbuildHome = (string)versionKey.GetValue("MSBuildToolsPath");
					var msbuildExe = Path.Combine(msbuildHome, "MSBuild.exe");

					if (_fileSystem.FileExists(msbuildExe))
						yield return new MSBuildInfo(new Version(versionName), msbuildExe);
				}
			}
		}
	}
}