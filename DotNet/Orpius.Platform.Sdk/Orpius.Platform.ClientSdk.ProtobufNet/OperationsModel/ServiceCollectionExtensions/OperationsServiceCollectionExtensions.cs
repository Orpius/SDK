using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

using Orpius.Platform.OperationsModel.RpcOperationsService;
using Orpius.Platform.RpcServiceModel;
using Orpius.Platform.RpcServices;
using ProtoBuf.Grpc.ClientFactory;

namespace Orpius.Platform.OperationsModel.ServiceCollectionExtensions
{
	public static class OperationsServiceCollectionExtensions
	{
		public static IServiceCollection AddOrpiusOperations(
			this IServiceCollection services,
			Func<Uri> getOrpiusServerAddress, bool dangerousAcceptAnyCertificate)
		{
			services.AddOperationsGrpcClient(getOrpiusServerAddress, dangerousAcceptAnyCertificate);
			services.AddSingleton<OperationsInterceptor>();

			return services;
		}

		static IServiceCollection AddOperationsGrpcClient(
			this IServiceCollection services,
			Func<Uri> getOrpiusServerAddress, bool dangerousAcceptAnyCertificate)
		{
			//var builder = services.AddCodeFirstGrpcClient<IOperationsService>(
			//						  options => { options.Address = getOrpiusServerAddress(); })
			//					  .AddInterceptor(() => new OperationInterceptor())
			//					  .ConfigurePrimaryHttpMessageHandler(CreateHandler);
			var builder = services.AddCodeFirstGrpcClient<IOperationsService>(
									  options =>
									  {
										  options.Address = getOrpiusServerAddress();
									  })
								  .AddInterceptor<OperationsInterceptor>();
			//.AddHttpMessageHandler<OperationsHeaderHandler>();

			if (dangerousAcceptAnyCertificate)
			{
				builder.ConfigurePrimaryHttpMessageHandler(DevelopmentHandlerFactory.CreateHandler);
			}

			return services;
		}
	}
}
