using CameraService;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly ILogger<ConfigurationController> _logger;
    private readonly IConfiguration _configuration;

    public ConfigurationController(ILogger<ConfigurationController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }


    [HttpGet("trackedobjects", Name = "GetTrackedObjects")]
    public IActionResult GetServices()
    {
        _logger.LogInformation("Getting tracked objects");
        return Ok(_configuration.GetSection("TrackedObjects").Get<List<string>>() ?? []);
    }
}
