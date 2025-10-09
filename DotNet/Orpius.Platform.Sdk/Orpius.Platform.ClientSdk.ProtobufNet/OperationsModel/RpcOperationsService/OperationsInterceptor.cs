using System;
using System.Collections.Generic;
using System.Linq;

using Grpc.Core;

using Orpius.Platform.Collections;
using Orpius.Platform.RpcServiceModel;

namespace Orpius.Platform.OperationsModel.RpcOperationsService
{
	public class OperationsInterceptor : InterceptorForHeadersBase
	{
		readonly MutableKeyIndex<Guid, IOperationsServiceParameters> keyIndex;

		/// <param name="parameters">
		/// The parameter objects that were added to the services collection
		/// with AddSingleton&lt;IOperationsServiceParameters&gt;</param>
		public OperationsInterceptor(IEnumerable<IOperationsServiceParameters> parameters)
		{
			IDictionary<Guid, IOperationsServiceParameters> dictionary 
				= parameters.ToDictionary(osp => osp.ExternalId);

			/* Wrap with a self-healing index that knows how
			   to read the current key from the value. */
			keyIndex = new MutableKeyIndex<Guid, IOperationsServiceParameters>(
							dictionary,
							osp => osp.ExternalId);
		}

		/// <inheritdoc />
		protected override CallOptions AddHeaders(CallOptions options, object request)
		{
			var provider = request as IOperationExternalIdProvider;

			if (provider == null)
			{
				throw new ArgumentException(
					$"The request must implement {nameof(IOperationExternalIdProvider)}.", nameof(request));
			}

			IOperationsServiceParameters osp = keyIndex.GetOrRepair(provider.OperationExternalId);

			Metadata meta = options.Headers ?? new Metadata();

			meta.Add("authorization", $"{OperationHeaders.ApiKey} {osp.ApiKey}");

			return options.WithHeaders(meta);
		}
	}
}
