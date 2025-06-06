using System;
using System.Collections.Generic;

namespace Orpius.Platform.Tooling
{
	public interface ICombinedContext
	{
		public object NativeContext { get; set; }

		IDictionary<string, string> SharedContext { get; set; }
	}

	public class CombinedContext : ICombinedContext
	{
		public CombinedContext(IDictionary<string, string> sharedDictionary, object? nativeContext)
		{
			this.SharedContext = sharedDictionary
								 ?? throw new ArgumentNullException(nameof(sharedDictionary));
			NativeContext = nativeContext;
		}

		public object?                     NativeContext { get; set; }
		public IDictionary<string, string> SharedContext { get; set; }
	}
}