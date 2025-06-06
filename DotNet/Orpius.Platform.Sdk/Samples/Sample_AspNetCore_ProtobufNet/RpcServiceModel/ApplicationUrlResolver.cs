using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Sample_AspNetCore_ProtobufNet.RpcServiceModel
{
	class ApplicationUrlResolver
	{
		bool orpiusHostedInDocker = true;

		public string GetApplicationUrl(IServiceProvider services)
		{
			// 1) Pull the raw address that Kestrel/ASP.NET Core is bound to:
			//    e.g. "https://[::]:7194" or "http://localhost:5000"
			string serverAddress = GetServerAddress(services)
								   ?? throw new InvalidOperationException(
									   "Unable to determine application URL: no addresses were reported.");

			if (!Uri.TryCreate(serverAddress, UriKind.Absolute, out Uri? parsed))
			{
				throw new InvalidOperationException($"Server address '{serverAddress}' is not a valid absolute URI.");
			}

			if (orpiusHostedInDocker)
			{
				return $"https://host.docker.internal:{parsed.Port}";
			}

			// 2) On Windows (including WSL2 host), ALWAYS use "localhost" as the host.
			//    On plain Linux, we try DNS for a real IPv4; but under WSL2, DNS gives you a WSL IP (10.x.x.x),
			//    which Windows cannot connect to. Instead, WSL forwards that port to Windows' localhost.
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || IsRunningUnderWsl())
			{
				UriBuilder builder = new(parsed)
				{
					Host = "localhost",
					Port = parsed.Port
				};
				return builder.Uri.ToString().TrimEnd('/');
			}

			// 3) Otherwise (pure Linux), try to resolve an IPv4 address via DNS:
			string? dnsIp = GetIPUsingDns();
			if (!string.IsNullOrWhiteSpace(dnsIp))
			{
				UriBuilder builder = new(parsed)
				{
					Host = dnsIp,
					Port = parsed.Port
				};
				return builder.Uri.ToString().TrimEnd('/');
			}

			// 4) Fallback: return whatever Kestrel reported (e.g. "https://0.0.0.0:7194" or "https://localhost:7194")
			return parsed.ToString().TrimEnd('/');
		}

		string? GetIPUsingDns()
		{
			try
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
				IPAddress? ipv4
					= hostEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
				return ipv4?.ToString();
			}
			catch
			{
				return null;
			}
		}

		string? GetServerAddress(IServiceProvider services)
		{
			IServer server = services.GetRequiredService<IServer>();
			IServerAddressesFeature? feature = server.Features.Get<IServerAddressesFeature>();
			return feature?.Addresses.FirstOrDefault();
		}

		bool IsRunningUnderWsl()
		{
			// A simple WSL detection: check if /proc/version exists and mentions "Microsoft"
			// (WSL2 uses a Linux kernel but /proc/version contains "Microsoft" in its text.)
			if (!File.Exists("/proc/version"))
			{
				return false;
			}

			string text = File.ReadAllText("/proc/version");
			return text.Contains("Microsoft", StringComparison.OrdinalIgnoreCase);
		}
	}
}