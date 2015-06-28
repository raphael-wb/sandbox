using System;
using Xunit;

namespace Rwb.PowerShell
{
	public class MSBuildInfoTests
	{
		[Fact]
		public void Constructor_ShouldValidateArguments()
		{
			Assert.Throws<ArgumentNullException>(() => new MSBuildInfo(null, @"T:\absolute-path"));
			Assert.Throws<ArgumentNullException>(() => new MSBuildInfo(new Version("1.0"), null));
			Assert.Throws<ArgumentException>(() => new MSBuildInfo(new Version("1.0"), ""));
			Assert.Throws<ArgumentException>(() => new MSBuildInfo(new Version("1.0"), "relative-path"));
		}
	}
}
