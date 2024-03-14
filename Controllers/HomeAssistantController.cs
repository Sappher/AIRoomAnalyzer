using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class HomeAssistantController : ControllerBase
{
    private readonly ILogger<HomeAssistantController> _logger;
    private readonly IServiceProvider _serviceprovider;
    private readonly HomeAssistantService _homeAssistantService;
    protected ServiceClient? _serviceClient;

    public HomeAssistantController(ILogger<HomeAssistantController> logger, IServiceProvider serviceprovider, HomeAssistantService homeAssistantService)
    {
        _logger = logger;
        _serviceprovider = serviceprovider;
        _homeAssistantService = homeAssistantService;

        _serviceClient = serviceprovider.GetService<ServiceClient>();
    }

    [HttpGet("setinputtextvalue", Name = "SetInputTextValue")]
    public async Task<IActionResult> SetInputTextValue(string entity_id, string text)
    {
        if (_serviceClient == null)
        {
            _logger.LogError("ServiceClient is null, cannot report to Home Assistant");
            return BadRequest();
        }
        await _serviceClient.CallService("input_text.set_value", new { entity_id, value = text });
        _logger.LogInformation($"Reported to Home Assistant: {entity_id} - {text}");
        return Ok();
    }

    [HttpGet("setdummyreport", Name = "SetDummyReport")]
    public async Task<IActionResult> SetDummyReport(string entity_id)
    {
        var report = new ImageAnalyzeReport
        {
            error = false,
            response = new AIRoomResponse
            {
                GeneralFeel = "Dummy feel at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            }
        };
        await _homeAssistantService.Report(entity_id, report);
        return Ok();
    }
}
