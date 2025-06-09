using Orpius.Platform.RpcServices;
using Orpius.Platform.Tooling;
using Orpius.Platform.Tooling.ToolRegistration;

using ProtoBuf.Grpc.ClientFactory;
using ProtoBuf.Grpc.Configuration;
using ProtoBuf.Grpc.Server;

using Sample_AspNetCore_ProtobufNet.Components;
using Sample_AspNetCore_ProtobufNet.RpcServiceModel;
using Sample_AspNetCore_ProtobufNet.Services;
using Sample_AspNetCore_ProtobufNet.ToolRegistration;

[assembly: GenerateToolRegistryItem("Sample_AspNetCore_ProtobufNet.ToolRegistration.SampleTools2")]

namespace Sample_AspNetCore_ProtobufNet
{
	public class Program
	{
		public static void Main(string[] args)
		{
			WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

			IServiceCollection services = builder.Services;

			services.AddRazorComponents().AddInteractiveServerComponents();

			services.AddGrpc();
			services.AddCodeFirstGrpc();

			/* Here we add two GRPC client interceptors,
			   allowing us to push the authentication headers. */
			services.AddCodeFirstGrpcClient<IOperationsService>(
						options => { options.Address = new Uri(ApplicationState.OrpiusServerUrl); })
					.AddInterceptor(() => new OperationInterceptor())
					.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
					{
						/* For use with self-signed certificate. Not for production. */
						ServerCertificateCustomValidationCallback
							= HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
					});

			services.AddCodeFirstGrpcClient<IToolsRegistrationService>(
						options => { options.Address = new Uri(ApplicationState.OrpiusServerUrl); })
					.AddInterceptor(() => new ToolsRegistrationInterceptor())
					.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
					{
						/* For use with self-signed certificate. Not for production. */
						ServerCertificateCustomValidationCallback
							= HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
					});

			builder.Services.AddScoped<RegistrationUtility>();

			/* For Orpius calling back to use tools. */

			/* This allows tools to be resolved using the IServiceProvider. */
			services.AddSingleton<IToolResolver>(sp => new ServiceProviderAdapter(sp));
			services.AddAssociatedSingletons<IToolRegistry, ToolRegistry>();

			/* ToolProviderService requires IToolCaller.
			   ToolRegistry also serves as the IToolCaller.
			   ToolProviderService is the GRPC service that receives 
			   requests from Orpius to use tools. */
			services.AddSingleton<IToolCaller>(sp => sp.GetRequiredService<ToolRegistry>());
			services.AddAssociatedSingletons<IToolProviderService, ToolProviderService>();

			/* The generated class in your project pulls in the IToolRegistry and registers itself. */
			services.AddSingleton<SampleTools2>();


			/* For the sample 'mobile' app. */
			services.AddAssociatedSingletons<IMyMobileAppService, MyMobileAppService>();

			services.AddSingleton(BinderConfiguration.Create(
				binder: new BinderFromServices(builder.Services)));
			

			WebApplication app = builder.Build();

			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
			}

			app.UseHttpsRedirection();
			app.UseAntiforgery();

			/* For Orpius calling back to use tools. */
			app.MapGrpcService<IToolProviderService>();

			/* For the 'mobile' app. */
			app.MapGrpcService<IMyMobileAppService>();

			app.MapStaticAssets();
			app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

			app.Run();
		}
	}

	static class ServiceCollectionExtensions
	{
		internal static void AddAssociatedSingletons<TInterface, TImplementation>(this IServiceCollection services)
			where TInterface : class
			where TImplementation : class, TInterface
		{
			services.AddSingleton<TImplementation>();
			services.AddSingleton<TInterface>(sp => sp.GetRequiredService<TImplementation>());
		}
	}
}