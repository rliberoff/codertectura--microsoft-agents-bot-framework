using Demo.Agents;
using Demo.Microsoft.Agents.Handlers;
using Demo.Plugins;

using Microsoft.Agents.Authentication;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.Agents.Protocols.Connector;
using Microsoft.Agents.Protocols.Primitives;

using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());

builder.Configuration.AddJsonFile($@"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($@"appsettings.{Environment.UserName}.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables()
                     ;


builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Register Semantic Kernel
builder.Services.AddKernel();

// Register the AI service of your choice. AzureOpenAI and OpenAI are demonstrated...
builder.Services.AddAzureOpenAIChatCompletion(
        deploymentName: builder.Configuration.GetSection(@"AIServices:AzureOpenAI").GetValue<string>(@"DeploymentName")!,
        endpoint: builder.Configuration.GetSection(@"AIServices:AzureOpenAI").GetValue<string>(@"Endpoint")!,
        apiKey: builder.Configuration.GetSection(@"AIServices:AzureOpenAI").GetValue<string>(@"ApiKey")!);

builder.Services.AddOptionsWithValidateOnStart<WeatherStackOptions>().Bind(builder.Configuration.GetSection(nameof(WeatherStackOptions))).ValidateDataAnnotations();

builder.Services.AddSingleton<IConnections, ConfigurationConnections>();
builder.Services.AddSingleton<IChannelServiceClientFactory, RestChannelServiceClientFactory>();
builder.Services.AddCloudAdapter();

builder.Services.AddTransient(sp =>
{
    return new WeatherForecastAgent(sp.GetRequiredService<Kernel>(), sp);
});
builder.Services.AddTransient<IBot, Bot>();

var app = builder.Build();

app.MapControllers();

await app.RunAsync();

await app.DisposeAsync();
