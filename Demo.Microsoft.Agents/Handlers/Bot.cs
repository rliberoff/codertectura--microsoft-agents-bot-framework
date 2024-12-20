using Demo.Agents;

using Microsoft.Agents.Protocols.Adapter;
using Microsoft.Agents.Protocols.Primitives;

namespace Demo.Microsoft.Agents.Handlers;

public sealed class Bot : ActivityHandler
{
    private readonly WeatherForecastAgent weatherAgent;

    public Bot(WeatherForecastAgent weatherAgent)
    {
        this.weatherAgent = weatherAgent;
    }

    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        // Invoke the WeatherForecastAgent to process the message
        var forecastResponse = await weatherAgent.InvokeAgentAsync(turnContext.Activity.Text);
     
        if (forecastResponse == null)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text(@"Sorry, I couldn't get the weather forecast at the moment!"), cancellationToken);
            return;
        }

        // Create a response message based on the response content type from the WeatherForecastAgent
        IActivity response = forecastResponse.ContentType switch
        {
            WeatherForecastAgentResponseContentType.AdaptiveCard => MessageFactory.Attachment(new Attachment()
            {
                ContentType = @"application/vnd.microsoft.card.adaptive",
                Content = forecastResponse.Content,
            }),
            _ => MessageFactory.Text(forecastResponse.Content),
        };

        // Send the response message back to the user. 
        await turnContext.SendActivityAsync(response, cancellationToken);
    }

    protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
    {
        // When someone (or something) connects to the bot, a MembersAdded activity is received.
        // For this sample,  we treat this as a welcome event, and send a message saying hello.
        // For more details around the membership lifecycle, please see the lifecycle documentation.
        IActivity message = MessageFactory.Text(@"Hello and Welcome! I'm here to help with all your weather forecast needs using the new Microsoft Agents SDK!");

        // Send the response message back to the user. 
        await turnContext.SendActivityAsync(message, cancellationToken);
    }
}
