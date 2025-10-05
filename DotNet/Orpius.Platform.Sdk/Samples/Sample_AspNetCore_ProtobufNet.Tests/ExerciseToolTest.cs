using System.Net;

using Grpc.Net.Client;

using Microsoft.AspNetCore.Mvc.Testing;
using Orpius.Platform.RpcServices;
using Orpius.Platform.Tooling.RpcToolProviderService;

using ProtoBuf.Grpc.Client;

namespace Sample_AspNetCore_ProtobufNet.Tests
{
	public class ExerciseToolTest : IClassFixture<WebApplicationFactory<Sample_AspNetCore_ProtobufNet.Program>>
	{
		readonly WebApplicationFactory<Sample_AspNetCore_ProtobufNet.Program> applicationFactory;

		public ExerciseToolTest(WebApplicationFactory<Sample_AspNetCore_ProtobufNet.Program> applicationFactory)
		{
			this.applicationFactory = applicationFactory;
		}

		[Fact]
		public async Task Get_EndpointsReturnSuccessAndCorrectContentType()
		{
			// Arrange: create an HttpClient that will send requests to our in-memory TestServer
			var client = applicationFactory.CreateClient();

			// Act: call your root endpoint (e.g. "/")
			var response = await client.GetAsync("/");

			// Assert: the response status code is 200 OK
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		}

		[Fact]
		public async Task UseTool_ReturnsExpectedResult()
		{
			// 1) Arrange: create an HttpClient that points at the in-memory TestServer.
			//    We give it a dummy BaseAddress ("http://localhost") so that
			//    GrpcChannel.ForAddress(...) will accept it, but all traffic actually
			//    goes through the TestServer's handler.
			var httpClient = applicationFactory.CreateClient(new WebApplicationFactoryClientOptions
			{
				BaseAddress = new Uri("http://localhost")
			});

			// 2) Wrap that HttpClient in a GrpcChannel. This allows us to call gRPC over
			//    the same in-memory server. Because TestServer does not do real TLS/HTTP2,
			//    Grpc.Net.Client will automatically downgrade to HTTP/1.1+ProtoBuf framing.
			var channel = GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
			{
				HttpClient = httpClient
			});

			// 3) Create a code-first gRPC client for IToolProviderService:
			var toolClient = channel.CreateGrpcService<IToolProviderService>();

			// 4) Build a test request. Fill in whatever Context / ToolName / etc. you need.
			var request = new UseToolRequest
			{
				Context = { ["someStateKey"] = "someStateValue" },
				ToolName = "FlightStatusChecker",
				ToolMember = "GetStatus",
				ParameterAsJson = "{\"FlightNumber\":123}"
			};

			// 5) Act: call the server's UseTool(...) method. This is calling your
			//    IToolProviderService implementation registered via MapGrpcService<IToolProviderService>().
			var response = await toolClient.UseTool(request);

			// 6) Assert: verify that you got back a non-null ToolResult (or whatever logic you expect).
			Assert.NotNull(response);
			//Assert.NotNull(response.ToolResult);

			// (Optional) If your service sets ReplacementContext, you can assert on that as well:
			// Assert.Contains("newStateKey", response.ReplacementContext);
		}
	}
}

