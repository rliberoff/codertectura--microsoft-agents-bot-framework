using Microsoft.Extensions.Options;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

using System.ComponentModel;

namespace Demo.Plugins;

public sealed class WeatherForecastPlugin
{
    private readonly IChatCompletionService chatCompletionService;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly WeatherStackOptions options;

    public WeatherForecastPlugin(Kernel kernel, IHttpClientFactory httpClientFactory, IOptions<WeatherStackOptions> options)
    {
        chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        this.httpClientFactory = httpClientFactory;

        this.options = options.Value;
    }

    [KernelFunction]
    [Description(@"Gets the current weather for the specified city")]
    public async Task<string> GetWeatherForCityAsync(string cityName, CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();

        using var request = new HttpRequestMessage(HttpMethod.Get, $@"https://api.weatherstack.com/current?query={cityName}&access_key={options.AccessKey}");
        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        var systemPrompt = $$"""
            You are an expert AI understanding and interpreting the JSON response from the WeatherStack service. Create a easily to read and short summary of the weather from the following JSON:
            
            {{content}}
            
            """;

        var result = await chatCompletionService.GetChatMessageContentAsync(systemPrompt, new AzureOpenAIPromptExecutionSettings()
        {
            MaxTokens = 200,
            Temperature = 0.1,
            TopP = 1.0,
        }, cancellationToken: cancellationToken);

        return result.Content!;
    }
}
