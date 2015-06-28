using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;

namespace Rwb.PowerShell
{
    [Cmdlet(VerbsLifecycle.Start, "AgnosticProcess"), OutputType(typeof(Process))]
    public class StartAgnosticProcessCmdlet : ProcessCmdletBase
    {
        [Alias("PSPath")]
        [Parameter(Mandatory = true, Position = 0), ValidateNotNullOrEmpty]
        public string FilePath { get; set; }

        [Alias("Args")]
        [Parameter(Position = 1), ValidateNotNullOrEmpty]
        public string[] ArgumentList { get; set; }

        protected override string GetFileName() => FilePath;
        protected override IEnumerable<string> GetArgumentList() => ArgumentList?.Select(CommandLineHelper.EscapeArgument);

		public StartAgnosticProcessCmdlet() : base(ServiceLocator.Get<IFileSystem>())
	    {
		}
	}
}
