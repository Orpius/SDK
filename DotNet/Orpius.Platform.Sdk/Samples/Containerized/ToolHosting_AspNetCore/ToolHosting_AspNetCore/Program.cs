namespace ToolHosting_AspNetCore
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddRazorPages();

			builder.Services.AddHttpClient<QuantumSimulatorClient>(
				client =>
				{
					string? baseUrl = builder.Configuration["QuantumSimulator:BaseUrl"];

					if (string.IsNullOrWhiteSpace(baseUrl))
					{
						throw new InvalidOperationException("QuantumSimulator:BaseUrl is not configured.");
					}

					client.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
				});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
