using System;
using System.Net.Http;

using Microsoft.Extensions.DependencyInjection;

using Orpius.Platform.RpcServiceModel;
using Orpius.Platform.RpcServices;
using ProtoBuf.Grpc.ClientFactory;

using Sample_AspNetCore_ProtobufNet.RpcServiceModel;

namespace Orpius.Platform.Tooling.ToolRegistration
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddToolRegistration(
			this IServiceCollection services)
		{
			/* We send the API Key and External ID to Orpius in the headers of the request.
			   This class relies on IToolRegistrationParameters.*/
			services.AddSingleton<ToolsRegistrationHeaderHandler>();

			/* This allows tools to be resolved using the IServiceProvider. */
			services.AddSingleton<IToolResolver>(sp => new ServiceProviderAdapter(sp));
			services.AddAssociatedSingletons<IToolRegistry, ToolRegistry>();

			/* ToolProviderService requires IToolCaller.
			   ToolRegistry also serves as the IToolCaller.
			   ToolProviderService is the GRPC service that receives
			   requests from Orpius to use tools. */
			services.AddSingleton<IToolCaller>(sp => sp.GetRequiredService<ToolRegistry>());
			services.AddAssociatedSingletons<IToolProviderService, ToolProviderService>();

			/* The IRegistrationMediator abstracts the use of the actual IToolRegistrationService. */
			services.AddSingleton<IRegistrationMediator, RegistrationMediator>();
			
			return services;
		}

		public static IServiceCollection WithAutomaticProviderRegistration(
			this IServiceCollection services)
		{
			services.AddHostedService<RegistrationHostedService>();

			return services;
		}

		public static IServiceCollection AddToolsRegistrationGrpcClient(
			this IServiceCollection services, 
			Func<Uri> getOrpiusServerAddress, bool dangerousAcceptAnyCertificate)
		{
			var builder = services.AddCodeFirstGrpcClient<IToolsRegistrationService>(
									  options => 
									  {
										  options.Address = getOrpiusServerAddress();
									  })
								  .AddHttpMessageHandler<ToolsRegistrationHeaderHandler>();

			if (dangerousAcceptAnyCertificate)
			{
				builder.ConfigurePrimaryHttpMessageHandler(CreateHandler);
			}

			return services;
		}

		public static IServiceCollection AddOperationsGrpcClient(
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
								  .AddHttpMessageHandler<OperationsHeaderHandler>();

			if (dangerousAcceptAnyCertificate)
			{
				builder.ConfigurePrimaryHttpMessageHandler(CreateHandler);
			}

			return services;
		}

		static HttpMessageHandler CreateHandler() => new HttpClientHandler
		{
			/* For use with self-signed certificate. Not for production. */
			ServerCertificateCustomValidationCallback
				= HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
		};

		internal static void AddAssociatedSingletons<TInterface, TImplementation>(this IServiceCollection services)
			where TInterface : class
			where TImplementation : class, TInterface
		{
			services.AddSingleton<TImplementation>();
			services.AddSingleton<TInterface>(sp => sp.GetRequiredService<TImplementation>());
		}
	}
}
