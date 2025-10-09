using System;
using System.Collections.Generic;
using System.Linq;

using Grpc.Core;

using Orpius.Platform.Collections;
using Orpius.Platform.RpcServiceModel;
using Orpius.Platform.Tooling.RpcToolsRegistrationService;

namespace Orpius.Platform.Tooling.ToolRegistration.RpcServiceModel
{
	public class ToolRegistrationInterceptor : InterceptorForHeadersBase
	{
		readonly MutableKeyIndex<Guid, IToolRegistrationParameters> keyIndex;

		public ToolRegistrationInterceptor(IEnumerable<IToolRegistrationParameters> parameters)
		{
			IDictionary<Guid, IToolRegistrationParameters> dictionary
				= parameters.ToDictionary(osp => osp.ToolsetExternalId);

			/* Wrap with a self-healing index that knows how
			   to read the current key from the value. */
			keyIndex = new MutableKeyIndex<Guid, IToolRegistrationParameters>(
				dictionary,
				osp => osp.ToolsetExternalId);
		}

		/// <inheritdoc />
		protected override CallOptions AddHeaders(CallOptions options, object request)
		{
			var externalIdOwner = request as IToolsetExternalIdProvider;

			if (externalIdOwner == null)
			{
				throw new ArgumentException(
					$"The request must implement {nameof(IToolsetExternalIdProvider)}.", nameof(request));
			}

			var trp = keyIndex.GetOrRepair(externalIdOwner.ToolsetExternalId);

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
