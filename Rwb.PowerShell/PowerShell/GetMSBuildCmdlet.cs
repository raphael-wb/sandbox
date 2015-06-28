using System;
using System.Management.Automation;

namespace Rwb.PowerShell
{
	[Cmdlet(VerbsCommon.Get, "MSBuild"), OutputType(typeof(MSBuildInfo))]
	public class GetMSBuildCmdlet : Cmdlet
	{
		[Parameter(Position = 0), ValidateNotNull]
		public Version Version { get; set; }

		private readonly MSBuildLocator _msBuildLocator;

		public GetMSBuildCmdlet() : this(ServiceLocator.Get<IFileSystem>()) { }

		public GetMSBuildCmdlet(IFileSystem fileSystem)
		{
			Require.NotNull(fileSystem, nameof(fileSystem));
			_msBuildLocator = new MSBuildLocator(fileSystem);
		}

		protected override void ProcessRecord()
		{
			if (Version == null)
			{
				WriteObject(_msBuildLocator.GetAll64(), true);
			}
			else
			{
				var msBuild = _msBuildLocator.GetVersion(Version);
				if (msBuild == null)
					throw new ItemNotFoundException(Resources.MSBuildVersionNotFoundError.FormatInvariant(Version));

				WriteObject(msBuild);
			}
		}
	}
}
