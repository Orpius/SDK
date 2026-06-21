using Microsoft.AspNetCore.Server.Kestrel.Core;

using Orpius.Platform.OperationsModel;
using Orpius.Platform.OperationsModel.ServiceCollectionExtensions;
using Orpius.Platform.RpcServices;
using Orpius.Platform.Tooling;
using Orpius.Platform.Tooling.RpcToolsRegistrationService;
using Orpius.Platform.Tooling.ToolRegistration;
using Orpius.Samples.RealEstate.RpcServiceModel;

using ProtoBuf.Grpc.Configuration;
using ProtoBuf.Grpc.Server;

// This attribute causes all classes decorated with [Tool] to be included
// in the tooling provided to your AI Agent. 
// Use the `GenerateToolRegistryItemAttribute.ScanAssembliesContaining` property 
// to selectively bring in tools from other projects.
[assembly: GenerateToolRegistryItem("Orpius.Samples.RealEstate.AllTools")]

namespace Orpius.Samples.RealEstate.Web
{
	public class ProgramForRealEstateSample
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			IServiceCollection services = builder.Services;

			OrpiusSampleOptions sampleOptions
				= SampleOptionsRetriever.GetOptions(builder.Configuration);

			services.AddSingleton(sampleOptions);
			services.AddSingleton<ApplicantDatabase>();
			services.AddSingleton<ApplicantRegistrar>();
			services.AddSingleton<ListingDatabase>();
			services.AddSingleton<PropertyLister>();
			services.AddSingleton<NotificationService>();
			services.AddSingleton<RealEstateAgentMessenger>();

			services.AddScoped<RealEstateConversationService>();
			services.AddScoped<ApplicantConversationService>();
			services.AddScoped<ListingConversationService>();

			services.AddScoped<
				IRealEstateAgentIdentityService,
				DemoIdentityService>();

			services.AddRazorPages();
			services.AddGrpc();
			services.AddCodeFirstGrpc();
			services.AddSingleton(BinderConfiguration.Create(
				binder: new BinderFromServices(services)));

			// We enable HTTP1 and HTTP2 support for development purposes,
			// enabling gRPC support over non-TLS channels.
			builder.WebHost.ConfigureKestrel(
				options => {
					options.ConfigureEndpointDefaults(
						e => { e.Protocols = HttpProtocols.Http1AndHttp2; });
				});

			// ------ Orpius Tooling (enabling your AI Agent to call your server to carry out tasks) ------

			// We provide one or more IToolRegistrationParameters instances,
			// which are used to register tools with the Orpius server.
			// NOTE: You can add multiple IToolRegistrationParameters instances.
			//       All are registered.
			FuncRegistrationParameters toolRegistrationParameters
				= new(getLocalUrl: () => sampleOptions.ToolsRegistration.IncomingUrl
										 ?? throw new InvalidOperationException(
											 "IncomingUrl must not be null."),
					getExternalId: () => sampleOptions.ToolsRegistration.ExternalId,
					getApiKey: () => sampleOptions.ToolsRegistration.AccessKey)
				{
					CallBackHeaders = new List<HeaderMessage>
					{
						// Headers are sent back to your application with each `UseTool` request,
						// allowing you to authenticate the Orpius server.
						// These are encrypted and stored securely by the Orpius system.
						new("MySecretHeader", "MyValue")
					}
				};
			services.AddSingleton<IToolRegistrationParameters>(toolRegistrationParameters);

			services.AddOrpiusToolRegistration(
						() => sampleOptions.OrpiusServerUrl
							?? throw new InvalidOperationException(
								"OrpiusServerUrl must not be null."),
						dangerousAcceptAnyCertificate: true)
					.WithAutomaticProviderRegistration();

			// ------ Orpius Operations ------

			FuncOperationsParameters funcOperationsParameters = new(
				() => sampleOptions.Operations.ExternalId,
				() => sampleOptions.Operations.ApiKey);

			services.AddSingleton<IOperationsServiceParameters>(funcOperationsParameters);

			services.AddOrpiusOperations(
				() => sampleOptions.OrpiusServerUrl
					  ?? throw new InvalidOperationException(
						  "OrpiusServerUrl must not be null."),
				dangerousAcceptAnyCertificate: true);

			// ------ Orpius tool implementations ------

			services.AddSingleton<Orpius.Samples.RealEstate.AllTools>();

			WebApplication app = builder.Build();

			/* We resolve the generated registry item so that it adds itself
			   to the IToolRegistry. */
			_ = app.Services.GetRequiredService<Orpius.Samples.RealEstate.AllTools>();

			/* This allows Orpius to call your server to use tools. */
			app.MapGrpcService<IToolProviderService>();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapRazorPages();

			app.Run();
		}
	}
}
