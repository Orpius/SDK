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
using Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius;

[assembly: GenerateToolRegistryItem("Sample_AspNetCore_ProtobufNet.ToolRegistration.SampleTools")]

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

			services.AddSingleton<IToolRegistrationParameters>(
				sp =>
				{
					Uri? uri = null;

					return new FuncRegistrationParameters(
						() => uri ??= new Uri(new ApplicationUrlResolver().GetApplicationUrl(sp)),
						() => ApplicationState.ToolsRegistrationSettings.ExternalId,
						() => Task.FromResult(ApplicationState.ToolsRegistrationSettings.AccessKey)
					);
				});

			services.AddSingleton<RegistrationHeaderHandler>();

			/* We add two GRPC interceptors,
			   allowing us to push the authentication headers. */
			
			services.AddCodeFirstGrpcClient<IToolsRegistrationService>(
						options => { options.Address = new Uri(ApplicationState.OrpiusServerUrl); })
					.AddHttpMessageHandler<RegistrationHeaderHandler>()
					.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
					{
						/* For use with self-signed certificate. Not for production. */
						ServerCertificateCustomValidationCallback
							= HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
					});

			services.AddCodeFirstGrpcClient<IOperationsService>(
						options => { options.Address = new Uri(ApplicationState.OrpiusServerUrl); })
					.AddInterceptor(() => new OperationInterceptor())
					.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
					{
						/* For use with self-signed certificate. Not for production. */
						ServerCertificateCustomValidationCallback
							= HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
					});

			services.AddHostedService<RegistrationHostedService>();

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

			/* The generated class in your project pulls
			   in the IToolRegistry and registers itself.
			   The tools are generated via the GenerateToolRegistryItemAttribute 
			   (at the top of this file).*/
			services.AddSingleton<SampleTools>();

			/* Add the tool implementations. The IToolRegistry uses the IToolResolver,
			   to locate the services when a UseToolRequest arrives. */
			services.AddSingleton<FlightStatusChecker>();
			services.AddSingleton<WeatherForecaster>();

			/* For the sample 'mobile' app. */
			services.AddAssociatedSingletons<IMyMobileAppService, MyMobileAppService>();

			services.AddSingleton(BinderConfiguration.Create(
				binder: new BinderFromServices(builder.Services)));

			WebApplication app = builder.Build();

			/* We must resolve the generated IToolsRegistryItem
			   so that it adds itself to the IToolRegistry. */
			var sampleTools = app.Services.GetRequiredService<SampleTools>();

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