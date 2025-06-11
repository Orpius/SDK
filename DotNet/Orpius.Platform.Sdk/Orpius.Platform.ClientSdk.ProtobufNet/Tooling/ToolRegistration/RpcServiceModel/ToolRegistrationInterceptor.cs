using System;
using System.Collections.Generic;
using System.Linq;

using Grpc.Core;

using Orpius.Platform.RpcServiceModel;
using Orpius.Platform.Tooling.RpcToolsRegistrationService;

namespace Orpius.Platform.Tooling.ToolRegistration.RpcServiceModel
{
	public class ToolRegistrationInterceptor : InterceptorForHeadersBase
	{
		readonly IDictionary<Guid, IToolRegistrationParameters> dictionary;

		public ToolRegistrationInterceptor(IEnumerable<IToolRegistrationParameters> parameters)
		{
			dictionary = parameters.ToDictionary(trp => trp.ToolsetExternalId);
		}

		protected override CallOptions AddHeaders(CallOptions options, object request)
		{
			var externalIdOwner = request as IToolsetExternalIdProvider;

			if (externalIdOwner == null)
			{
				throw new ArgumentException(
					$"The request must implement {nameof(IToolsetExternalIdProvider)}.", nameof(request));
			}

			if (!dictionary.TryGetValue(externalIdOwner.ToolsetExternalId, out IToolRegistrationParameters? trp))
			{
				throw new ArgumentException(
					$"No tool registration parameters found for toolset ID: {externalIdOwner.ToolsetExternalId}",
					nameof(IToolsetExternalIdProvider.ToolsetExternalId));
			}

			Metadata meta = options.Headers ?? new Metadata();
			meta.Add(ToolsRegistrationHeaders.ExternalId,
				trp.ToolsetExternalId.ToString());

			meta.Add("authorization", $"{ToolsRegistrationHeaders.ApiKey} {trp.ApiKey}");

			var registerAsProviderRequest = request as RegisterAsProviderRequest;

			if (registerAsProviderRequest != null)
			{
				foreach (var header in trp.CallBackHeaders)
				{
					registerAsProviderRequest.Headers.Add(header);
				}
			}

			return options.WithHeaders(meta);
		}
	}
}
