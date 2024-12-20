using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;

using Microsoft.Bot.Connector.Authentication;

namespace Demo.Microsoft.Bots.Adapters;

public class BotCloudAdapterWithErrorHandler : CloudAdapter
{
    public BotCloudAdapterWithErrorHandler(BotFrameworkAuthentication auth) : base(auth)
    {
        OnTurnError = async (turnContext, exception) =>
        {
            Console.WriteLine($@"[OnTurnError] unhandled error: {exception.Message}");
            await turnContext.SendActivityAsync(MessageFactory.Text(@"The bot encountered an error or bug. Please try again later."), cancellationToken: default);
            await turnContext.TraceActivityAsync("OnTurnError Trace", exception.Message, "https://www.botframework.com/schemas/error", "TurnError");
        };
    }
}
