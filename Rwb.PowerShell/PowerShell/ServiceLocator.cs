namespace Rwb.PowerShell
{
	internal static class ServiceLocator
	{
		internal static T Get<T>() where T : class
		{
			return PowerShellContainerManager.Get<T>();
		}
	}
}
