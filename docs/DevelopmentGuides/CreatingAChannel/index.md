# Setting up Tool Support

To allow Orpius to call your tools, Orpius needs to know the **publicly reachable URL** of your server.
Since development machines usually run on `localhost`, we recommend creating a secure tunnel.

There are multiple options, but one of the easiest ways to get started 
is with **Cloudflare's [TryCloudflare](https://developers.cloudflare.com/cloudflare-one/connections/connect-networks/trycloudflare/)** service.

---

## Step 1 – Export a Development TLS Certificate

Orpius communicates over HTTPS. We need to export the development certificate used by ASP.NET Core.

1. Create a directory to store the exported certificate.

	```powershell
	New-Item -ItemType Directory -Force -Path "C:\Dev\Certificates\ExportedDev" | Out-Null
	```

2. Export the certificate:

	```powershell
	dotnet dev-certs https --export-path "C:\Dev\Certificates\ExportedDev\aspnet-dev.pem" --format Pem -p ""
	```

---

## Step 2 – Install Cloudflare Tunnel (cloudflared)

1. Install `cloudflared` on Windows using **winget**:

	```powershell
	winget install --id Cloudflare.cloudflared
	```

   For macOS/Linux, follow the [Cloudflare downloads page](https://developers.cloudflare.com/cloudflare-one/connections/connect-networks/downloads/).

2. Restart your terminal session after installation.

---

## Step 3 – Run the Tunnel

Launch the tunnel to your local application (default port 7194).

```powershell
cloudflared tunnel --url https://localhost:7194 --no-tls-verify --http2-origin=false
```

You should see output like this:

```
2025-09-29T11:55:15Z INF Requesting new quick Tunnel on trycloudflare.com...
2025-09-29T11:55:21Z INF +--------------------------------------------------------------------------------------------+
2025-09-29T11:55:21Z INF |  Your quick Tunnel has been created! Visit it at (it may take some time to be reachable):  |
2025-09-29T11:55:21Z INF |  https://insertion-behaviour-airfare-suites.trycloudflare.com                              |
2025-09-29T11:55:21Z INF +--------------------------------------------------------------------------------------------+
```

The URL shown (ending in `trycloudflare.com`) is now your **public development URL**.
Use this URL when configuring your tools in Orpius.

---

## Step 4 – Using the Tunnel URL in Orpius

* Take the generated tunnel URL (e.g. `https://insertion-behaviour-airfare-suites.trycloudflare.com`).
* Register it as the endpoint for your tools in the **Orpius Console**.
* Orpius will now securely send requests to your local server through the tunnel.

---

## Step 5 – Persistent Tunnels (Optional)

The quick tunnel above will change each time you start it.
To create a **persistent tunnel** with a fixed URL, see:
[Cloudflare documentation on named tunnels](https://developers.cloudflare.com/cloudflare-one/connections/connect-networks/configure-tunnels/)

---

## Next Steps

* Explore the **sample code** in this repository.
* Add your own tools and expose them via the tunnel.
* Integrate the Orpius SDK into your existing application.

---

## Resources

* [Orpius Website](https://orpius.com)
* [Cloudflare Tunnels Documentation](https://developers.cloudflare.com/cloudflare-one/connections/connect-networks/)
* [.NET Development Certificates](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-dev-certs)

