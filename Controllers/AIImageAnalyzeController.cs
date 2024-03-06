using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;

namespace AIProxy.Controllers;

[ApiController]
[Route("[controller]")]
public class AIImageAnalyzeController : ControllerBase
{
    private readonly ILogger<AIImageAnalyzeController> _logger;
    private readonly IServiceProvider _serviceprovider;
    private readonly IPromptsService _promptsService;

    public AIImageAnalyzeController(ILogger<AIImageAnalyzeController> logger, IServiceProvider serviceprovider, IPromptsService promptsService)
    {
        _logger = logger;
        _serviceprovider = serviceprovider;
        _promptsService = promptsService;
    }



    [HttpPost(Name = "AnalyzeImage")]
    public async Task<IActionResult> Post(string room, IFormFile image)
    {
        try
        {
            var imageData = new byte[image.Length];
            await image.OpenReadStream().ReadAsync(imageData);
            var ai = _serviceprovider.GetRequiredService<IOpenAIService>();
            var messages = _promptsService.AllPrompts.ToList();
            messages.Add(ChatMessage.FromUser(
                new List<MessageContent>
                {
                    MessageContent.TextContent("Analyze this room. Focus on finding all furniture and objects that you can. Also try to analyze the general feel of the room. If you find alerting things happening in the room, please let me know."),
                    MessageContent.ImageBinaryContent(
                        binaryImage: imageData,
                        imageType: "image/jpeg",
                        detail: "auto"
                    )

                }
            ));
            var response = await ai.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest()
            {
                Model = Models.Gpt_4_vision_preview,
                MaxTokens = 300,
                Messages = messages,
                N = 1
            });


            if (response.Successful == false)
                throw new Exception($"Error from AI, HttpStatusCode: {response.HttpStatusCode}, error: {response.Error?.Message}");

            string ResponseJson = response.Choices.First()?.Message?.Content ?? "";
            // Remove ```json and ``` from the response
            ResponseJson = ResponseJson.Replace("```json", "").Replace("```", "");
            Console.WriteLine(ResponseJson);
            if (ResponseJson == "")
                throw new Exception("Empty or no response from AI");
            else
            {
                var RoomResponse = JsonConvert.DeserializeObject<AIRoomResponse>(ResponseJson);
                if (RoomResponse == null)
                    throw new Exception($"Failed to deserialize response from AI, response was: {ResponseJson}");
                RoomResponse.RoomName = room;
                return Ok(RoomResponse);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
