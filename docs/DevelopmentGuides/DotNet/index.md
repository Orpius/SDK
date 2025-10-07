# Getting Started with the SDK

1. **Clone this repository**  
	```powershell
	git clone https://github.com/Orpius/SDK.git
	````

2. **Open the solution** in **Visual Studio 2022** (or later).

3. **Build and run** the sample application to confirm everything is working locally.
   By default, the sample project runs on:

	```
	https://localhost:7194
	```

---

If you haven't already, please read the [Operations section](../../UserGuide/Operations/)
of the user guide.

The [Orpius SDK repository](https://github.com/Orpius/SDK) contains varios sample projects.
This section focusses on the *Sample_AspNetCore_ProtobufNet* project 
and the *Sample_MobileApp_ProtobufNet* project.
The ASP.NET Core sample shows how to set up your own web application so that 
it can commicate with the Orpius server.
The MobileApp project demonstrates how you might create a mobile or desktop
app that communicates directly with your web API application, which relays
communication from Orpius. See below.

![Comminication from Mobile App to Web API to Orpius](Images/MobileToWebApiToOrpius_Small.png)
*Comminication from Mobile App to Web API to Orpius*

The Orpius SDK for .NET consists of a class library, 
*Orpius.Platform.ClientSdk.ProtobufNet*, which contains the types 
for creating and handling requests to and from the Orpius server;
and an Analyzer project, *Orpius.Platform.ClientSdk.ProtobufNet.Generators* 
that makes it easy to automatically generate 
nearly everything you need to provide your own custom APIs (tools) 
for your AI Agents to use.

The aforementioned libraries are both available as the following NuGet packages:

* [Orpius.Platform.ClientSdk.ProtobufNet](https://www.nuget.org/packages/Orpius.Platform.ClientSdk.ProtobufNet)
* [Orpius.Platform.ClientSdk.ProtobufNet.Generators](https://www.nuget.org/packages/Orpius.Platform.ClientSdk.ProtobufNet.Generators)

> **Note:** Both libraries are currently available as pre-release packages, 
but will transition to release in the near future.

In this section, we look at enabling Operations. Operations allow your
middle-tier application (such as web application) to communicate with an Orpius server;
they allow you incorporate your custom AI agents into your own applications.

If you haven't done so already, create a new Operation in the Orpius client app.
See [Operations](../../UserGuide/Operations/Operations.md).

You'll need the *External ID* and *Access Key 1* to connect your application to Orpius.

## Setting up your middle-tier Application

In the SDK sample, the *Sample_AspNetCore_ProtobufNet* is a consise example
of the key parts that you are likely to have in your project.

It references the *Orpius.Platform.ClientSdk.ProtobufNet*, and *Orpius.Platform.ClientSdk.ProtobufNet.Generators* project.
You, however, will likely want to reference the NuGet packages mentioned above instead.

The `Program` class is the entry point for the application.

The first thing you may notice, is at the top of the file we have:

```cs
[assembly: GenerateToolRegistryItem("Sample_AspNetCore_ProtobufNet.ToolRegistration.SampleTools")]
```

This assembly level attribute `Orpius.Platform.Tooling.GenerateToolRegistryItemAttribute` instructs
the incremental code generator, located in the *Orpius.Platform.ClientSdk.ProtobufNet.Generators*
project, to generate the code representing the API surface of your custom tools. 
We explore custom tools, later in the document.

Communication with Orpius is done using the Google's [Protocol Buffers](https://protobuf.dev/) a.k.a., protobuf;
and Google's *gRPC* (remote procedure call) framework.
[Protobuf-net](https://github.com/protobuf-net/protobuf-net) is also used for code-first support.
In the following excerp, we use the extension methods afforded 
by these libraries to bring-in protobuf support.

```cs
services.AddGrpc();
services.AddCodeFirstGrpc();
services.AddSingleton(BinderConfiguration.Create(
	binder: new BinderFromServices(builder.Services)));
```

Following the protobuf initialization, we see a section related to custom tools.
We'll skip over that for now, and return to it later in this document.

Further down in the `Main` method of the `Program` class we see 
the section beginning with 'Orpius Operations'.
It is here that we provide the 'ExternalId' and the 'ApiKey' values from the operation:

```cs
FuncOperationsParameters funcOperationsParameters = new(
	() => ApplicationState.OperationsSettings.ExternalId,
	() => ApplicationState.OperationsSettings.ApiKey);
```

The `FuncOperationsParameters` class implements the `Orpius.Platform.OperationsModel.IOperationsServiceParameters` interface,
and provides a convenient way to pass the operation details to the Orpius subsystem.

We add the `funcOperationsParameters` object to the ASP.NET Core's *services* `IServiceCollection`:

```cs
services.AddSingleton<IOperationsServiceParameters>(funcOperationsParameters);
```

> **Note:** If you'd like to use more than one Operation in your application, 
you can by calling `services.AddSingleton<IOperationsServiceParameters>(anotherObject);`.

The `Program` class contains a method that retrieves the URL of the Orpius server; see below.

```cs
static Uri GetOrpiusServerUri() => new(ApplicationState.OrpiusServerUrl);
```

URLs of this type usually begin with a unique identifier (GUID) assigned to your organisation or environment.

**Format:**  
`https://{guid}.app.orpius.com`

**Example:**  
`https://fscnry5cyy3myzh55kky4jmgjx.app.orpius.com`



