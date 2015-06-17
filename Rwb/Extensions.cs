using System;
using System.Globalization;

namespace Rwb
{
	public static class Extensions
	{
		[Pure]
		[StringFormatMethod("format")]
		public static string FormatInvariant(this string format, params object[] args)
		{
			return string.Format(CultureInfo.InvariantCulture, format, args);
		}

		public static T GetService<T>(this IServiceProvider serviceProvider)
		{
			Require.NotNull(serviceProvider, nameof(serviceProvider));

			return (T)serviceProvider.GetService(typeof(T));
		}
	}
}
