using System.ComponentModel.DataAnnotations;

namespace Demo.Plugins;

public sealed class WeatherStackOptions
{
    /// <summary>
    /// Gets the WeatherStack API access key.
    /// </summary>
    [Required]
    public required string AccessKey { get; init; }
}
