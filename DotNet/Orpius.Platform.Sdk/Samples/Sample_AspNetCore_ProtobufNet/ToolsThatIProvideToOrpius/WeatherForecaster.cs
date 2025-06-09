#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using Orpius.Platform.Tooling;

namespace Sample_AspNetCore_ProtobufNet.ToolsThatIProvideToOrpius
{
	[Tool(Name = "WeatherForecast")]
	public class WeatherForecaster
	{
		[ToolMethod(Name = "GetForecast", Description = "Retrieve the forecast for the specified ")]
		public async Task<GetForecastResponse> GetForecastAsync(GetForecastRequest request, ICombinedContext context)
		{
			var forecastDate = request.ForecastDate;
			var dayOfWeek = forecastDate.DayOfWeek;

			var response = new GetForecastResponse();

			switch (dayOfWeek)
			{
				case DayOfWeek.Monday:
					response.Conditions  = WeatherConditions.Overcast;
					response.TemperatureInCelsius = 12;
					break;
				case DayOfWeek.Tuesday:
					response.Conditions  = WeatherConditions.Rain;
					response.TemperatureInCelsius = 9;
					break;
				case DayOfWeek.Wednesday:
					response.Conditions  = WeatherConditions.Sunny;
					response.TemperatureInCelsius = 18;
					break;
				case DayOfWeek.Thursday:
					response.Conditions  = WeatherConditions.Overcast;
					response.TemperatureInCelsius = 14;
					break;
				case DayOfWeek.Friday:
					response.Conditions  = WeatherConditions.Sunny;
					response.TemperatureInCelsius = 20;
					break;
				case DayOfWeek.Saturday:
					response.Conditions  = WeatherConditions.Snow;
					response.TemperatureInCelsius = -1;
					break;
				case DayOfWeek.Sunday:
					response.Conditions  = WeatherConditions.Rain;
					response.TemperatureInCelsius = 7;
					break;
			}

			return response;
		}

		[ToolMethod(Description = "Suggests clothing based on the forecast for the specified date.")]
		public async Task<GetClothingRecommendationResponse> GetClothingRecommendation(
			GetClothingRecommendationRequest request, 
			ICombinedContext context)
		{
			var forecast = await GetForecastAsync(new GetForecastRequest {ForecastDate = request.DateTime}, context);

			var recommendation = new ClothingRecommendation
			{
				Outerwear = forecast.TemperatureInCelsius < 5  ? "Heavy coat" :
							forecast.TemperatureInCelsius < 15 ? "Jacket" : "Light sweater",
				Accessories = forecast.Conditions == WeatherConditions.Rain  ? "Umbrella" :
							  forecast.Conditions == WeatherConditions.Sunny ? "Sunglasses" : "None"
			};

			return new GetClothingRecommendationResponse
			{
				ForecastSummary = $"Expect {forecast.Conditions} with {forecast.TemperatureInCelsius}°C.",
				Recommendation  = recommendation
			};
		}
	}

	public class GetForecastRequest
	{
		[ToolStringProperty(RepresentAs = typeof(string),
			OpenApiFormat = "date-time",
			Description = "The date and time for which the weather forecast is requested.",
			Required = true)]
		public DateTime ForecastDate { get; set; }

		/// <summary>
		/// Because this property is not decorated
		/// with a ToolPropertyAttribute, or ToolStringPropertyAttribute,
		/// it is not part of the API surface for the tool.
		/// </summary>
		public bool AnIgnoredProperty { get; set; }
	}

	public class GetForecastResponse
	{
		[ToolProperty(Name = "WeatherConditions")]
		public WeatherConditions Conditions { get; set; }

		[ToolProperty]
		public int TemperatureInCelsius { get; set; }
	}

	public enum WeatherConditions
	{
		Sunny,
		Overcast,
		Rain,
		Snow
	}

	public class GetClothingRecommendationRequest
	{
		[ToolStringProperty(RepresentAs = typeof(string),
			OpenApiFormat = "date-time",
			Description = "The date and time for which the clothing recommendation is requested.",
			Required = true)]
		public DateTime DateTime { get; set; }

		/// <summary>
		/// Because this property is not decorated
		/// with a ToolPropertyAttribute, or ToolStringPropertyAttribute,
		/// it is not part of the API surface for the tool.
		/// </summary>
		public bool AnIgnoredProperty { get; set; }
	}

	public class GetClothingRecommendationResponse
	{
		[ToolProperty]
		public string? ForecastSummary { get; set; }

		[ToolProperty]
		public ClothingRecommendation? Recommendation { get; set; }
	}

	public class ClothingRecommendation
	{
		[ToolProperty]
		public string? Outerwear { get; set; }

		[ToolProperty]
		public string? Accessories { get; set; }
	}
}
