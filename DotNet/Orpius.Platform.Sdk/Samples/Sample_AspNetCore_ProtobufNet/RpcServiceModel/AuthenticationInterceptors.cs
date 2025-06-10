using Grpc.Core;
using Grpc.Core.Interceptors;

using Orpius.Platform.OperationsModel.RpcOperationsService;

namespace Sample_AspNetCore_ProtobufNet.RpcServiceModel
{
	public abstract class InterceptorForHeadersBase : Interceptor
	{
		protected abstract CallOptions AddHeaders(CallOptions options);

		public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
			TRequest request,
			ClientInterceptorContext<TRequest, TResponse> context,
			AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
		{
			return continuation(request,
				new ClientInterceptorContext<TRequest, TResponse>(
					context.Method, context.Host, AddHeaders(context.Options)));
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
					context.Method, context.Host, AddHeaders(context.Options)));
		}
	}

	public sealed class OperationInterceptor : InterceptorForHeadersBase
	{
		protected override CallOptions AddHeaders(CallOptions options)
		{
			Metadata meta = options.Headers ?? new Metadata();
			meta.Add(OperationHeaders.ExternalId,
				ApplicationState.OperationsSettings.ExternalId.ToString());
			meta.Add(OperationHeaders.AccessKey,
				ApplicationState.OperationsSettings.AccessKey.ToString());
			return options.WithHeaders(meta);
		}
	}

	//public sealed class ToolsRegistrationInterceptor : InterceptorForHeadersBase
	//{
	//	protected override CallOptions AddHeaders(CallOptions options)
	//	{
	//		Metadata meta = options.Headers ?? new Metadata();
	//		meta.Add(ToolsRegistrationHeaders.ExternalId,
	//			ApplicationState.ToolsRegistrationSettings.ExternalId.ToString());
	//		meta.Add(ToolsRegistrationHeaders.AccessKey,
	//			ApplicationState.ToolsRegistrationSettings.AccessKey.ToString());
	//		return options.WithHeaders(meta);
	//	}
	//}

	//public sealed class ToolsRegistrationInterceptor : InterceptorForHeadersBase
	//{
	//	readonly IToolRegistrationParameters registrationHeaders;

	//	public ToolsRegistrationInterceptor(IToolRegistrationParameters registrationParameters)
	//	{
	//		this.registrationHeaders = registrationParameters 
	//								   ?? throw new ArgumentNullException(nameof(registrationParameters));
	//	}

	//	protected override CallOptions AddHeaders(CallOptions options)
	//	{
	//		Metadata meta = options.Headers ?? new Metadata();
	//		using var token = registrationHeaders.GetAccessTokenAsync().GetAwaiter().GetResult();
	//		var headerValue = Convert.ToBase64String(token.GetRawBytes());

	//		meta.Add(ToolsRegistrationHeaders.AccessKey, headerValue);
	//		meta.Add(ToolsRegistrationHeaders.ExternalId,
	//			registrationHeaders.ExternalId.ToString());

	//		return options.WithHeaders(meta);
	//	}
	//}
}