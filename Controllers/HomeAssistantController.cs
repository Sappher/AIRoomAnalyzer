using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("[controller]")]
public class HomeAssistantController : ControllerBase
{
    private readonly ILogger<HomeAssistantController> _logger;
    private readonly IServiceProvider _serviceprovider;
    private readonly StatesClient _statesClient;

    public HomeAssistantController(ILogger<HomeAssistantController> logger, IServiceProvider serviceprovider, StatesClient statesClient)
    {
        _logger = logger;
        _serviceprovider = serviceprovider;
        _statesClient = statesClient;
    }

    [HttpGet(Name = "GetState")]
    public async Task<IActionResult> Post(string entityId)
    {
        var state = await _statesClient.GetState(entityId);
        if (state == null)
            return NotFound();
        else
            return Ok(state);
    }
}
