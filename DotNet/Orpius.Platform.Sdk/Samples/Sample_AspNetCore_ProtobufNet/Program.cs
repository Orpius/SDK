using Orpius.Platform.RpcServiceModel;
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

			services.AddToolRegistration().WithAutomaticProviderRegistration();

			/* We add two GRPC interceptors,
			   allowing us to include the authentication headers. */

			/* Tools - They allow your AI Agent to call your server to carry out tasks. */
			services.AddCodeFirstGrpcClient<IToolsRegistrationService>(
						options => { options.Address = new Uri(ApplicationState.OrpiusServerUrl); })
					.AddHttpMessageHandler<RegistrationHeaderHandler>()
					.ConfigurePrimaryHttpMessageHandler(CreateHandler);

			/* Operations - They allow your application to communicate with an AI Agent. */
			services.AddCodeFirstGrpcClient<IOperationsService>(
						options => { options.Address = new Uri(ApplicationState.OrpiusServerUrl); })
					.AddInterceptor(() => new OperationInterceptor())
					.ConfigurePrimaryHttpMessageHandler(CreateHandler);

			/* The generated class in your project pulls in the `IToolRegistry`
			   and registers itself. The tools are generated 
			   via the `GenerateToolRegistryItemAttribute` (at the top of this file). */
			services.AddSingleton<SampleTools>();

			/* Add your tool implementations.
			   The IToolRegistry uses the IToolResolver, to locate the services 
			   when a `UseToolRequest` arrives. */
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

			/* This allows Orpius to call your server to use tools.
			   You may want to add authentication to this service.
			   You can use the `RegisterAsProviderRequest.Headers` property 
			   to provide headers that are stored securely on the Orpius server, 
			   and provided back to your server during an `IToolProviderService.UseTool` call. */
			app.MapGrpcService<IToolProviderService>();

			/* For the sample 'mobile' app. */
			app.MapGrpcService<IMyMobileAppService>();

			app.MapStaticAssets();
			app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

			app.Run();
		}

		static HttpMessageHandler CreateHandler() => new HttpClientHandler
		{
			/* For use with self-signed certificate. Not for production. */
			ServerCertificateCustomValidationCallback 
				= HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
		};
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