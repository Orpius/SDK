using System.Collections.Generic;
using System.Linq;

namespace Orpius.Platform.Collections
{
	static class EnumerableExtensions
	{
		/// <summary>
		/// Returns the source sequence if it's non‐null;
		/// otherwise, returns Enumerable.Empty{T}.
		/// </summary>
		public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T>? source)
			=> source ?? Enumerable.Empty<T>();
	}
}
