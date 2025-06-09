// ReSharper disable RedundantUsingDirective
using System;
using System.Collections.Generic;

using ProtoBuf;

namespace Orpius.Platform.Tooling.RpcToolsRegistrationService
{
	partial class DeregisterAsProviderRequest
	{
#if !NET7_0_OR_GREATER
		public DeregisterAsProviderRequest(string providerUrl)
		{
			if (!Uri.TryCreate(providerUrl, UriKind.Absolute, out _))
			{
				throw new ArgumentException("Invalid URL." + providerUrl, nameof(providerUrl));
			}

			ProviderUrl = providerUrl;
		}
#endif

		[ProtoMember(1, IsRequired = true)]
#if NET7_0_OR_GREATER
		public required string ProviderUrl { get; set; }
#else
		public string ProviderUrl { get; set; }
#endif
	}
}
