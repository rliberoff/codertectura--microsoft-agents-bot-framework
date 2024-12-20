using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Demo.Plugins;

public sealed class AdaptiveCardPlugin
{
    private const string Instructions = 
        """
        When given data about the weather forecast for a given time and place, please generate an adaptive card
        that displays the information in a visually appealing way. Make sure to only return the valid adaptive card
        JSON string in the response.
        """;

    [KernelFunction]
    public static async Task<string> GetAdaptiveCardForData(Kernel kernel, string data)
    {
        // Create a chat history with the instructions as a system message and the data as a user message
        ChatHistory chat = new(Instructions)
        {
            new ChatMessageContent(AuthorRole.User, data)
        };

        // Invoke the model to get a response
        var chatCompletion = kernel.GetRequiredService<IChatCompletionService>();
        var response = await chatCompletion.GetChatMessageContentAsync(chat);

        return response.ToString();
    }
}
