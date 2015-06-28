using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Rwb.PowerShell
{
	[Cmdlet(VerbsLifecycle.Start, "MSBuild"), OutputType(typeof(Process))]
	public class StartMSBuildCmdlets : ProcessCmdletBase
	{
		[Parameter(Position = 0)]
		public string Project { get; set; }

		[Parameter(Position = 1), ValidateNotNullOrEmpty]
		public string[] Target { get; set; }

		[Parameter, ValidateNotNull]
		public Version Version { get; set; }

		[Parameter, ValidateRange(1, int.MaxValue)]
		public int MaxCpuCount { get; set; }

		[Parameter]
		public SwitchParameter NoLogo { get; set; }

		[Parameter]
		public IDictionary Property { get; set; }

		[Parameter]
		public Version ToolsVersion { get; set; }

		[Parameter, ValidateSet("quiet", "minimal", "normal", "detailed", "diagnostic", IgnoreCase = true)]
		public string Verbosity { get; set; }

		[Parameter]
		public SwitchParameter NoConsoleLogger { get; set; }

		[Parameter]
		public string LogFile { get; set; }

		private readonly MSBuildLocator _msBuildLocator;

		public StartMSBuildCmdlets() : this(ServiceLocator.Get<IFileSystem>()) { }

		private StartMSBuildCmdlets(IFileSystem fileSystem) : base(fileSystem)
		{
			Require.NotNull(fileSystem, nameof(fileSystem));
			_msBuildLocator = new MSBuildLocator(fileSystem);
		}

		protected override string GetFileName()
		{
			var msBuild = Version == null
				? _msBuildLocator.GetLatest()
				: _msBuildLocator.GetVersion(Version);

			if (msBuild != null)
				return msBuild.Path;

			throw new ItemNotFoundException(Version == null
				? "Unable to find any 64bits MSBuild on local machine"
				: Resources.MSBuildVersionNotFoundError.FormatInvariant(Version));
		}

		protected override IEnumerable<string> GetArgumentList()
		{
			if (MaxCpuCount > 1)
				yield return "/maxcpucount:" + MaxCpuCount;

			if (NoLogo)
				yield return "/nologo";

			if (Property != null && Property.Count > 0)
				yield return FormatPropertyArgument();

			if (Target != null && Target.Any())
				yield return "/target:" + string.Join(";", Target);

			if (ToolsVersion != null)
				yield return "/toolsversion:" + ToolsVersion;

			if (Verbosity == null && IsVerboseSwitchPresent())
				Verbosity = "diagnostic";
			if (Verbosity != null)
				yield return "/verbosity:" + Verbosity;

			if (NoConsoleLogger)
				yield return "/noconsolelogger";

			if (!string.IsNullOrEmpty(LogFile))
			{
				LogFile = this.GetFileSystemProviderPathFromPSPath(LogFile);
				yield return "/logger:FileLogger,Microsoft.Build.Engine;logfile=" + CommandLineHelper.EscapeArgument(LogFile);
			}

			if (!string.IsNullOrEmpty(Project))
				yield return CommandLineHelper.EscapeArgument(Project);
		}

		private bool IsVerboseSwitchPresent()
		{
			if (!MyInvocation.BoundParameters.ContainsKey("Verbose"))
				return false;
			return (SwitchParameter)MyInvocation.BoundParameters["Verbose"];
		}

		private string FormatPropertyArgument()
		{
			var sb = new StringBuilder();

			sb.Append("/property:");

			bool first = true;
			foreach (var pair in EnumerateDictionaryParameter(Property, nameof(Property)))
			{
				if (!first)
					sb.Append(';');
				else
					first = false;

				sb.Append(pair.Key).Append('=').Append(CommandLineHelper.EscapeArgument(pair.Value.ToString()));
			}

			return sb.ToString();
		}
	}
}
