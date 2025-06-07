using System.Diagnostics.CodeAnalysis;
using Orpius.Platform.Tooling.ToolRegistration;

namespace Sample_AspNetCore_ProtobufNet.Tooling
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
			tool = serviceProvider.GetService<T>();

			return tool is not null;
		}
	}
}
