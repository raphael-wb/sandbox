using System.Text.RegularExpressions;

namespace Rwb.PowerShell
{
	internal static class CommandLineHelper
	{
		private static readonly Regex s_doubleQuoteEscapingRegex = new Regex(@"(\\*)""");
		private const string DoubleQuoteEscapingReplacementPattern = @"\$1$0";
		private static readonly Regex s_argumentContainingSpaceEscapingRegex = new Regex(@"^(.*[\s;].*?)(\\*)$");
		private const string ArgumentContainingSpaceReplacementPattern = "\"$1$2$2\"";

		internal static string EscapeArgument(string argument)
		{
			Require.NotNull(argument, nameof(argument));

			if (argument.Length == 0)
				return "\"\"";
			argument = s_doubleQuoteEscapingRegex.Replace(argument, DoubleQuoteEscapingReplacementPattern);
			return s_argumentContainingSpaceEscapingRegex.Replace(argument, ArgumentContainingSpaceReplacementPattern);
		}
	}
}