using Microsoft.AspNetCore.Mvc;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;

namespace AIProxy.Controllers;

[ApiController]
[Route("[controller]")]
public class AIChatController : ControllerBase
{
    private readonly ILogger<AIChatController> _logger;
    private readonly IServiceProvider _serviceprovider;
    private readonly IPromptsService _promptsService;

    public AIChatController(ILogger<AIChatController> logger, IServiceProvider serviceprovider, IPromptsService promptsService)
    {
        _logger = logger;
        _serviceprovider = serviceprovider;
        _promptsService = promptsService;
    }



    [HttpGet(Name = "ChatWithAI")]
    public async Task<IActionResult> Post(string message, string room)
    {
        var ai = _serviceprovider.GetRequiredService<IOpenAIService>();
        var messages = _promptsService.AllPrompts.ToList();
        messages.Add(ChatMessage.FromUser(message));
        var response = await ai.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest()
        {
            Model = Models.Gpt_3_5_Turbo,
            MaxTokens = 150,
            Messages = messages
        });

        Console.WriteLine(response);

        if (response.Successful == false)
            return BadRequest($"Error from AI, HttpStatusCode: {response.HttpStatusCode}, error: {response.Error?.Message}");
        else
        {
            if (response.Choices.Count == 0)
                return BadRequest("No response from AI.");
            else
                return Ok(response.Choices.First()?.Message?.Content ?? "No response from AI.");
        }
    }
}
