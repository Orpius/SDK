using System;
using System.Collections.Generic;
using System.Linq;

using Grpc.Core;

using Orpius.Platform.RpcServiceModel;

namespace Orpius.Platform.OperationsModel.RpcOperationsService
{
	public class OperationsInterceptor : InterceptorForHeadersBase
	{
		readonly IDictionary<Guid, IOperationsServiceParameters> dictionary;

		public OperationsInterceptor(IEnumerable<IOperationsServiceParameters> parameters)
		{
			dictionary = parameters.ToDictionary(trp => trp.ExternalId);
		}

		protected override CallOptions AddHeaders(CallOptions options, object request)
		{
			var provider = request as IOperationExternalIdProvider;

			if (provider == null)
			{
				throw new ArgumentException(
					$"The request must implement {nameof(IOperationExternalIdProvider)}.", nameof(request));
			}

			if (!dictionary.TryGetValue(provider.OperationExternalId, out IOperationsServiceParameters? trp))
			{
				throw new ArgumentException(
					$"No operation parameters found for external ID: {provider.OperationExternalId}",
					nameof(IOperationExternalIdProvider.OperationExternalId));
			}

			Metadata meta = options.Headers ?? new Metadata();
			//meta.Add(OperationHeaders.ExternalId,
			//	trp.ExternalId.ToString());

			meta.Add("authorization", $"{OperationHeaders.ApiKey} {trp.ApiKey}");

			return options.WithHeaders(meta);
		}
	}
}
