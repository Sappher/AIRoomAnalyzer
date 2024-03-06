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
builder.Services.AddOpenAIService();
builder.Services.AddTransient<IHomeAssistantService, HomeAssistantService>();
builder.Services.AddSingleton<IPromptsService, PromptsService>();
builder.Services.AddSingleton<IImageAnalyzerService, ImageAnalyzerService>();

var haApikey = builder.Configuration["HomeAssistant:ApiKey"];
var haUrl = builder.Configuration["HomeAssistant:Url"];

ClientFactory.Initialize(haUrl, haApikey);
builder.Services.AddScoped(_ => ClientFactory.GetClient<StatesClient>());
builder.Services.AddSingleton(_ => ClientFactory.GetClient<ServiceClient>());

builder.Services.Configure<List<CameraServiceConfiguration>>(builder.Configuration.GetSection("Cameras"));
builder.Services.AddSingleton<CameraServiceFactory>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<CameraServiceFactory>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
