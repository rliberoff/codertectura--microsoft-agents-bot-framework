using Microsoft.AspNetCore.Mvc;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace DDemo.Microsoft.Bots.Controllers;

// ASP.Net Controller that receives incoming HTTP requests from the Azure Bot Service or other configured event activity protocol sources.
// When called, the request has already been authorized and credentials and tokens validated.
[ApiController]
[Route(@"api/messages")]
public class BotController(IBotFrameworkHttpAdapter adapter, IBot bot) : ControllerBase
{
    [HttpPost]
    public Task PostAsync(CancellationToken cancellationToken) => adapter.ProcessAsync(Request, Response, bot, cancellationToken);

}
