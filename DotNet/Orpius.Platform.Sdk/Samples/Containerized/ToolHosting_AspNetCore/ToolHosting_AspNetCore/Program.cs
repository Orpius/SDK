using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Orpius.Platform.Tooling;
using Orpius.Platform.Tooling.RpcToolsRegistrationService;
using Orpius.Platform.Tooling.ToolRegistration;
using ToolHosting_AspNetCore.ToolForOrpius;
using ToolHosting_AspNetCore.ToolsForOrpius;

[assembly: GenerateToolRegistryItem("ToolHosting_AspNetCore.ToolForOrpius.AllTools")]

namespace ToolHosting_AspNetCore
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			var services = builder.Services;

			/* Persist keys so cookies/tokens stay decryptable across restarts */
			services.AddDataProtection()
					.PersistKeysToFileSystem(new DirectoryInfo("/app/dp-keys"))
					.SetApplicationName("ToolHosting_AspNetCore");

			services.AddRazorPages(options =>
								   {
									   // Disable antiforgery token validation for simplicity in this example.
									   options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
								   });

			services.AddHttpClient<QuantumSimulatorClient>(
				client =>
				{
					string? baseUrl = builder.Configuration["QuantumSimulator:BaseUrl"];

					if (string.IsNullOrWhiteSpace(baseUrl))
					{
						throw new InvalidOperationException("QuantumSimulator:BaseUrl is not configured.");
					}

					client.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
				});

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
				= new(getLocalUrl: () => new Uri("https://alpine-remarkable-grown-possible.trycloudflare.com"),
					getExternalId: () => Guid.Parse("ee2b90ff-a4c6-44bf-93a7-a25b7e3271b0"),
					getApiKey: () => Guid.Parse("72e1b1f1-414b-46d9-bcb1-1a736d7e6027"))
				{
				};
			services.AddSingleton<IToolRegistrationParameters>(toolRegistrationParameters);

			services.AddOrpiusToolRegistration(() => new Uri("https://localhost:32774"),
						dangerousAcceptAnyCertificate: true)
					.WithAutomaticProviderRegistration();

			// The generated AllTools class contains all tools defined in this assembly.
			// It's generated because of the GenerateToolRegistryItem attribute at the top of this file.
			services.AddSingleton<AllTools>();
			// We also need to register each tool implementation.
			services.AddSingleton<QuantumQasm3Tool>();

			var app = builder.Build();

			// We must resolve the generated IToolsRegistryItem
			// so that it adds itself to the IToolRegistry.
			_ = app.Services.GetRequiredService<AllTools>();

			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.MapStaticAssets();
			app.MapRazorPages()
			   .WithStaticAssets();

			app.Run();
		}
	}
}
