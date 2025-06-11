using Grpc.Core;
using Grpc.Core.Interceptors;

using Orpius.Platform.OperationsModel.RpcOperationsService;

namespace Orpius.Platform.RpcServiceModel
{
	public abstract class InterceptorForHeadersBase : Interceptor
	{
		protected abstract CallOptions AddHeaders(CallOptions options, object request);

		public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
			TRequest request,
			ClientInterceptorContext<TRequest, TResponse> context,
			AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
		{
			return continuation(request,
				new ClientInterceptorContext<TRequest, TResponse>(
					context.Method, context.Host, AddHeaders(context.Options, request)));
		}

		public override AsyncServerStreamingCall<TResponse>
			AsyncServerStreamingCall<TRequest, TResponse>(
				TRequest request,
				ClientInterceptorContext<TRequest, TResponse> context,
				AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
		{
			return continuation(
				request,
				new ClientInterceptorContext<TRequest, TResponse>(
					context.Method, context.Host, AddHeaders(context.Options, request)));
		}
	}

	//public sealed class OperationInterceptor : InterceptorForHeadersBase
	//{
	//	protected override CallOptions AddHeaders(CallOptions options)
	//	{
	//		Metadata meta = options.Headers ?? new Metadata();
	//		meta.Add(OperationHeaders.ExternalId,
	//			ApplicationState.OperationsSettings.ExternalId.ToString());
	//		meta.Add(OperationHeaders.AccessKey,
	//			ApplicationState.OperationsSettings.AccessKey.ToString());
	//		return options.WithHeaders(meta);
	//	}
	//}
}