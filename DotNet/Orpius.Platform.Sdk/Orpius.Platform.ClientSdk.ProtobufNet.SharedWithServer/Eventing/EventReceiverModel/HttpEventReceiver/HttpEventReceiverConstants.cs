// ReSharper disable RedundantUsingDirective
using System;

namespace Orpius.Platform.Eventing.EventReceiverModel.HttpEventReceiver
{
	public static class HttpEventReceiverConstants
	{
		const string eventReceiverResource = "event";
		public const string EventReceiverApi = ApiConstants.ApiV1 + "/" + eventReceiverResource;
		public const string ReceiveEventAction = "raise";
		public const string EventIdParameter = "event-id";

		public static string GetReceiveEventUrl(Guid accessKey)
		{
			return $"{EventReceiverApi}/{ReceiveEventAction}?{EventIdParameter}={accessKey:N}";
		}
	}
}