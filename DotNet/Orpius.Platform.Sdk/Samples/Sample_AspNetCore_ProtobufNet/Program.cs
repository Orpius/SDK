using Microsoft.AspNetCore.Server.Kestrel.Core;

using Orpius.Platform.OperationsModel;
using Orpius.Platform.OperationsModel.ServiceCollectionExtensions;
using Orpius.Platform.RpcServices;
using Orpius.Platform.Tooling;
using Orpius.Platform.Tooling.RpcToolsRegistrationService;
using Orpius.Platform.Tooling.ToolRegistration;

using ProtoBuf.Grpc.Configuration;
using ProtoBuf.Grpc.Server;

using Sample_AspNetCore_ProtobufNet.Components;
using Sample_AspNetCore_ProtobufNet.Services;
using Sample_AspNetCore_ProtobufNet.ToolRegistration;
using Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius;

/* This attribute causes all classes decorated with [Tool] to be included
   in the tooling provided to your AI Agent. 
   Use the `GenerateToolRegistryItemAttribute.ScanAssembliesContaining` property 
   to selectively bring in tools from other projects. */
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
			services.AddSingleton(BinderConfiguration.Create(
				binder: new BinderFromServices(builder.Services)));

			/* We enable HTTP1 and HTTP2 support for development purposes,
			   enabling gRPC support over non-TLS channels. */
			builder.WebHost.ConfigureKestrel(
				options => { options.ConfigureEndpointDefaults(
					e => { e.Protocols = HttpProtocols.Http1AndHttp2; }); });

			/* ------ Orpius Tooling (enabling your AI Agent to call your server to carry out tasks) ------ */

			/* We provide one or more IToolRegistrationParameters instances,
			   which are used to register tools with the Orpius server.
			   NOTE: You can add multiple IToolRegistrationParameters instances.
					 All are registered. */
			FuncRegistrationParameters toolRegistrationParameters
				= new(getLocalUrl: () => ApplicationState.ToolsRegistrationSettings.IncomingUrl,
					getExternalId: () => ApplicationState.ToolsRegistrationSettings.ExternalId,
					getApiKey: () => ApplicationState.ToolsRegistrationSettings.AccessKey)
				{
					CallBackHeaders = new List<HeaderMessage>
					{
						/* Headers are sent back to your application with each `UseTool` request,
						   allowing you to authenticate the Orpius server.
						   These are encrypted and stored securely by the Orpius system. */
						new("MySecretHeader", "MyValue")
					}
				};
			services.AddSingleton<IToolRegistrationParameters>(toolRegistrationParameters);

			services.AddOrpiusToolRegistration(GetOrpiusServerUri, 
						dangerousAcceptAnyCertificate: true)
					.WithAutomaticProviderRegistration();

			/* ------ Orpius Operations (chatting with your AI Agent) ------ */

			FuncOperationsParameters funcOperationsParameters = new(
				() => ApplicationState.OperationsSettings.ExternalId,
				() => ApplicationState.OperationsSettings.ApiKey);

			/* NOTE: You can add multiple IOperationsServiceParameters instances. */
			services.AddSingleton<IOperationsServiceParameters>(funcOperationsParameters);

			services.AddOrpiusOperations(GetOrpiusServerUri,
				dangerousAcceptAnyCertificate: true);

			/* The generated class in your project pulls in the `IToolRegistry`
			   and registers itself. The tools are generated 
			   via the `GenerateToolRegistryItemAttribute` (at the top of this file). */
			services.AddSingleton<SampleTools>();

			/* Add your tool implementations.
			   This allows them to be resolved when requested by an AI agent.
			   NOTE: For tools to be available during a chat session, 
			         they must be specified in the ChatRequest. 
			         See `MyMobileAppService` for an example. */
			services.AddSingleton<FlightStatusChecker>();
			services.AddSingleton<WeatherForecaster>();

			/* For the sample 'mobile' app. */
			services.AddAssociatedSingletons<IMyMobileAppService, MyMobileAppService>();

			WebApplication app = builder.Build();

			/* We must resolve the generated IToolsRegistryItem
			   so that it adds itself to the IToolRegistry. */
			_ = app.Services.GetRequiredService<SampleTools>();
			
			/* This allows Orpius to call your server to use tools.
			   You may want to add authentication to this service.
			   You can use the `IToolRegistrationParameters.Headers` property, 
			   or the `RegisterAsProviderRequest.Headers` property directly,
			   to provide headers that are stored securely on the Orpius server. 
			   These are provided back to your server during an `IToolProviderService.UseTool` call. */
			app.MapGrpcService<IToolProviderService>();

			/* For the sample 'mobile' app. */
			app.MapGrpcService<IMyMobileAppService>();

			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
			}

			app.UseHttpsRedirection();
			app.UseAntiforgery();

			app.MapStaticAssets();
			app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

			app.Run();
		}

		/// <summary>
		/// ApplicationState is a demonstration-only static class
		/// that is populated using the Components/Pages/Home.razor page.
		/// </summary>
		static Uri GetOrpiusServerUri() => new(ApplicationState.OrpiusServerUrl);
	}
}