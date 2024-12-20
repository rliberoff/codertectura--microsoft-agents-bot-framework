using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Demo.Agents;

public sealed class WeatherForecastAgentResponse
{
    [JsonPropertyName(@"contentType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public WeatherForecastAgentResponseContentType ContentType { get; init; }

    [JsonPropertyName(@"content")]
    [Description(@"The content of the response, may be plain text, or JSON based adaptive card but must be a string.")]
    public required string Content { get; init; }
}
