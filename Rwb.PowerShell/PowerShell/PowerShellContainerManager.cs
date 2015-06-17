using System;

namespace Rwb.PowerShell
{
	public static class PowerShellContainerManager
	{
		private class PowerShellServiceProvider : IServiceProvider
		{
			private readonly IFileSystem _fileSystem = new FileSystem();
			private readonly IPathResolver _pathResolver = new PathResolver();

			public object GetService(Type serviceType)
			{
				if (serviceType == typeof(IFileSystem))
					return _fileSystem;
				if (serviceType == typeof(IPathResolver))
					return _pathResolver;
				return null;
			}
		}

		private static readonly IServiceProvider s_container = new PowerShellServiceProvider();
		private static IServiceProvider s_testContainer;

		public static T Get<T>() where T : class
		{
			return (s_testContainer ?? s_container).GetService<T>();
		}

		internal static void StartOverridingServiceLocatorForTests(IServiceProvider serviceLocator)
		{
			if (s_testContainer != null)
				throw new InvalidOperationException("Current test service locator wasn't clean-up since previous override");
			s_testContainer = serviceLocator;
		}

		internal static void StopOverridingServiceLocatorForTests()
		{
			if (s_testContainer == null)
				throw new InvalidOperationException("Test service locator was already clean-up");
			s_testContainer = null;
		}
	}
}