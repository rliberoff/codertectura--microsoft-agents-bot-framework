﻿using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.Agents.Protocols.Primitives;

using Microsoft.AspNetCore.Mvc;

namespace Demo.Microsoft.Agents.Controllers;

// ASP.Net Controller that receives incoming HTTP requests from the Azure Bot Service or other configured event activity protocol sources.
// When called, the request has already been authorized and credentials and tokens validated.
[ApiController]
[Route(@"api/messages")]
public class BotController(IBotHttpAdapter adapter, IBot bot) : ControllerBase
{
    [HttpPost]
    public Task PostAsync(CancellationToken cancellationToken) => adapter.ProcessAsync(Request, Response, bot, cancellationToken);

}
