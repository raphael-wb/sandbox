using System;
using Xunit;

namespace Rwb.PowerShell
{
	public class CommandLineHelperTests
	{
		[Fact]
		public void EscapeArgument_Null_ShouldThrow()
		{
			Assert.Throws<ArgumentNullException>(() => CommandLineHelper.EscapeArgument(null));
		}

		[Theory]
		[InlineData(@"", @"""""")]
		[InlineData(@"foo", @"foo")]
		[InlineData(@" ", @""" """)]
		[InlineData(@" foo", @""" foo""")]
		[InlineData(@"""foo""", @"\""foo\""")]
		[InlineData(@"foo\bar", @"foo\bar")]
		[InlineData(@"foo\bar\\", @"foo\bar\\")]
		[InlineData(@"foo bar\", @"""foo bar\\""")]
		[InlineData(@"foo bar\\", @"""foo bar\\\\""")]
		[InlineData(@"foo bar\\""", @"""foo bar\\\\\""""")]
		public void EscapeArgument(string argument, string expectedEscapedArgument)
		{
			string actual = CommandLineHelper.EscapeArgument(argument);
			Assert.Equal(expectedEscapedArgument, actual);
		}
	}
}
