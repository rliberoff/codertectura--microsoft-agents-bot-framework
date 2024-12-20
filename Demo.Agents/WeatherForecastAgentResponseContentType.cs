using System.Text.Json.Serialization;

namespace Demo.Agents;

public enum WeatherForecastAgentResponseContentType
{
    [JsonPropertyName(@"text")]
    Text,

    [JsonPropertyName(@"adaptive-card")]
    AdaptiveCard
}
