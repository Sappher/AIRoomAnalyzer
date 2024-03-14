using Newtonsoft.Json;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;

public class ImageAnalyzerService : IImageAnalyzerService
{
    private readonly ILogger<ImageAnalyzerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IPromptsService _promptsService;
    private readonly HomeAssistantService _homeAssistantService;

    public ImageAnalyzerService(ILogger<ImageAnalyzerService> logger, IServiceProvider serviceProvider, IPromptsService promptsService, HomeAssistantService homeAssistantService)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _promptsService = promptsService;
        _homeAssistantService = homeAssistantService;
    }

    public async Task<ImageAnalyzeReport> AnalyzeImage(byte[] data, CancellationToken? cancellationToken)
    {
        try
        {
            if (data.Length <= 0) throw new Exception("Empty image data");
            var ai = _serviceProvider.GetRequiredService<IOpenAIService>();
            var messages = _promptsService.AllPrompts.ToList();
            messages.Add(ChatMessage.FromUser(
                new List<MessageContent>
                {
                    MessageContent.TextContent("Analyze this room. Focus on finding all furniture and objects that you can. Also try to analyze the general feel of the room. If you find alerting things happening in the room, please let me know."),
                    MessageContent.ImageBinaryContent(
                        binaryImage: data,
                        imageType: "image/jpeg",
                        detail: "auto"
                    )

                }
            ));

            var response = await ai.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Model = Models.Gpt_4_vision_preview,
                MaxTokens = 300,
                Messages = messages,
                N = 1
            }, cancellationToken: cancellationToken ?? CancellationToken.None);

            if (response == null) throw new Exception("NULL response from AI");
            if (response.Successful == false) throw new Exception($"Unsuccessfull response from AI: {response.Error?.Message}");
            if (!response.Choices.Any()) throw new Exception("No choices in the AI response?");
            var responseStr = response.Choices.First().Message.Content ?? "";
            if (responseStr.Length <= 0) throw new Exception("Empty response from AI");
            var roomResponse = JsonConvert.DeserializeObject<AIRoomResponse>(responseStr.Replace("```json", "").Replace("```", ""));
            if (roomResponse == null) throw new Exception($"Could not convert AI response to AIRoomResponse, raw response was: {responseStr}");

            var report = new ImageAnalyzeReport { error = false, image = data, response = roomResponse };
            _logger.LogInformation($"Analyzed image, general feel in  {roomResponse.RoomName} is {roomResponse.GeneralFeel}");

            // Report to HA. Temp method for now
            _ = Task.Run(() => _homeAssistantService.Report("input_text.camera1", report));

            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing image");
            return new ImageAnalyzeReport { image = data, error = true };
        }
    }
}