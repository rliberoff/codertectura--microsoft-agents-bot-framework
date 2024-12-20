#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0110

using Demo.Plugins;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

using System.Text;
using System.Text.Json;

namespace Demo.Agents;

public sealed class WeatherForecastAgent
{
    private int retryCount;

    private readonly Kernel kernel;
    private readonly ChatHistory chatHistory;
    private readonly ChatCompletionAgent agent;

    private const string AgentName = @"WeatherForecastAgent";
    private const string AgentInstructions = """
            You are a friendly assistant that helps people find a weather forecast for a given time and place.
            You may ask follow up questions until you have enough information to answer the customers question,
            but once you have a forecast, make sure to format it nicely using an adaptive card.

            Respond in JSON format with the following JSON schema:
            
            {
                "contentType": "'Text' or 'AdaptiveCard' only",
                "content": "{The content of the response, may be plain text, or JSON based adaptive card}"
            }
            """;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherForecastAgent"/> class.
    /// </summary>
    /// <param name="kernel">An instance of <see cref="Kernel"/> for interacting with an LLM.</param>
    public WeatherForecastAgent(Kernel kernel, IServiceProvider serviceProvider)
    {
        this.kernel = kernel;
        this.chatHistory = [];

        // Define the agent
        this.agent =
            new()
            {
                Instructions = AgentInstructions,
                Name = AgentName,
                Kernel = this.kernel,
                Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                    ResponseFormat = @"json_object"
                }),
            };

        // Give the agent some tools to work with
        this.agent.Kernel.Plugins.Add(KernelPluginFactory.CreateFromType<DateTimePlugin>());
        this.agent.Kernel.Plugins.Add(KernelPluginFactory.CreateFromType<WeatherForecastPlugin>(serviceProvider: serviceProvider));
        this.agent.Kernel.Plugins.Add(KernelPluginFactory.CreateFromType<AdaptiveCardPlugin>());
    }

    /// <summary>
    /// Invokes the agent with the given input and returns the response.
    /// </summary>
    /// <param name="input">A message to process.</param>
    /// <returns>An instance of <see cref="WeatherForecastAgentResponse"/></returns>
    public async Task<WeatherForecastAgentResponse> InvokeAgentAsync(string input)
    {
        ChatMessageContent message = new(AuthorRole.User, input);
        this.chatHistory.Add(message);

        StringBuilder sb = new();
        await foreach (var response in this.agent.InvokeAsync(this.chatHistory))
        {
            this.chatHistory.Add(response);
            sb.Append(response.Content);
        }

        // Make sure the response is in the correct format and retry if necessary
        try
        {
            var resultContent = sb.ToString();
            var result = JsonSerializer.Deserialize<WeatherForecastAgentResponse>(resultContent)!;
            this.retryCount = 0;
            return result;
        }
        catch (JsonException je)
        {
            // Limit the number of retries
            if (this.retryCount > 2)
            {
                throw;
            }

            // Try again, providing corrective feedback to the model so that it can correct its mistake
            this.retryCount++;
            return await InvokeAgentAsync($@"That response did not match the expected format. Please try again. Error: {je.Message}");
        }
    }
}

#pragma warning restore SKEXP0010
#pragma warning restore SKEXP0110
