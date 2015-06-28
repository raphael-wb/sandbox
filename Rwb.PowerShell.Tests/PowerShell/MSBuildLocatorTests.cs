using System;
using System.IO;
using System.Runtime.Remoting;
using Moq;
using Xunit;

namespace Rwb.PowerShell
{
	public class MSBuildLocatorTests
	{
		[Fact]
		public void GetAll64_ShouldFindAtLeastOne()
		{
			var target = new MSBuildLocator(new FileSystem());

			var actual = target.GetAll64();

			// Assumes current machine has at least 1 MSBuild installed
			Assert.NotEmpty(actual);
			
			foreach (var msBuild in actual)
				Assert.True(File.Exists(msBuild.Path));
		}

		[Fact]
		public void GetAll64_FilesDoNotExist_ShouldDiscardThem()
		{
			var fileSystem = new Mock<IFileSystem>();
			fileSystem.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(false);

			var target = new MSBuildLocator(fileSystem.Object);

			var actual = target.GetAll64();
			Assert.Empty(actual);
		}

		[Fact]
		public void GetLatest()
		{
			var target = new MSBuildLocator(new FileSystem());

			var actual = target.GetLatest();

			Assert.NotNull(actual);

			foreach(var msBuild in target.GetAll64())
				Assert.InRange(msBuild.Version, new Version(), actual.Version);
		}

		[Fact]
		public void GetVersion()
		{
			var target = new MSBuildLocator(new FileSystem());

			var latest = target.GetLatest();
			var actual = target.GetVersion(latest.Version);
			
			Assert.NotNull(actual);
			Assert.Equal(latest.Version, actual.Version);
		}
	}
}
