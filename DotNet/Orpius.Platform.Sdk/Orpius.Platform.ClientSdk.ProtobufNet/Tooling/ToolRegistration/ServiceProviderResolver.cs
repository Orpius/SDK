using System;
using System.Diagnostics.CodeAnalysis;

namespace Orpius.Platform.Tooling.ToolRegistration
{
	public class ServiceProviderAdapter : IToolResolver
	{
		readonly IServiceProvider serviceProvider;

		public ServiceProviderAdapter(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider 
				?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		public bool TryGetTool<T>([NotNullWhen(true)] out T? tool)
			where T : class
		{
			tool = (T)serviceProvider.GetService(typeof(T));

			return tool != null;
		}
	}
}
