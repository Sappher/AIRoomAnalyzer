using CameraService;
using CameraService.Configuration;
using HADotNet.Core;
using HADotNet.Core.Clients;
using OpenAI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add OpenAI service
var openAiOrg = builder.Configuration.GetSection("OpenAI")["Organization"] ?? "";
var openAiApiKey = builder.Configuration.GetSection("OpenAI")["ApiKey"] ?? "";

if (string.IsNullOrWhiteSpace(openAiOrg) || string.IsNullOrWhiteSpace(openAiApiKey))
{
    throw new Exception("OpenAI Organization and/or ApiKey not found in configuration");
}

builder.Services.AddOpenAIService(settings =>
{
    settings.Organization = openAiOrg;
    settings.ApiKey = openAiApiKey;
});

// Add PromtsService and ImageAnalyzerService
builder.Services.AddSingleton<IPromptsService, PromptsService>();
builder.Services.AddSingleton<IImageAnalyzerService, ImageAnalyzerService>();

// Add HomeAssistantService. Even if it's not enabled, add it to the container so that the service can be injected into other services
var haEnabled = bool.Parse(builder.Configuration.GetSection("HomeAssistant")["Enabled"] ?? "false");
if (haEnabled)
{
    var haApikey = builder.Configuration.GetSection("HomeAssistant")["Apikey"];
    var haUrl = builder.Configuration.GetSection("HomeAssistant")["Url"];
    if (string.IsNullOrWhiteSpace(haApikey) || string.IsNullOrWhiteSpace(haUrl))
    {
        throw new Exception("HomeAssistant Apikey and/or Url not found in configuration");
    }

    ClientFactory.Initialize(haUrl, haApikey);
    //builder.Services.AddScoped(_ => ClientFactory.GetClient<StatesClient>());
    builder.Services.AddSingleton(_ => ClientFactory.GetClient<ServiceClient>());
}

builder.Services.AddSingleton<HomeAssistantService>();

builder.Services.Configure<List<CameraServiceConfiguration>>(builder.Configuration.GetSection("Cameras"));
builder.Services.AddSingleton<CameraServiceFactory>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<CameraServiceFactory>());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
