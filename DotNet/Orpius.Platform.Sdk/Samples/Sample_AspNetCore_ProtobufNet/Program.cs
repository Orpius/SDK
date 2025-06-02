using Grpc.Core;

using Orpius.Platform.RpcServices;

using ProtoBuf.Grpc.ClientFactory;
using ProtoBuf.Grpc.Configuration;
using ProtoBuf.Grpc.Server;

using Sample_AspNetCore_ProtobufNet.Components;
using Sample_AspNetCore_ProtobufNet.RpcServiceModel;
using Sample_AspNetCore_ProtobufNet.Services;
using Sample_AspNetCore_ProtobufNet.ToolRegistration;

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

			//services.AddTransient<OperationInterceptor>();

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

			services.AddSingleton<MyMobileAppService>();
			services.AddSingleton<IMyMobileAppService>(
				sp => sp.GetRequiredService<MyMobileAppService>());

			services.AddSingleton(BinderConfiguration.Create(
				binder: new BinderFromServices(builder.Services)));

			WebApplication app = builder.Build();

			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
			}

			app.UseHttpsRedirection();
			app.UseAntiforgery();

			app.MapGrpcService<IMyMobileAppService>();

			app.MapStaticAssets();
			app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

			app.Run();
		}
	}
}