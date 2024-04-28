using CameraService;
using HADotNet.Core.Clients;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("[controller]")]
public class CameraServiceController : ControllerBase
{
    private readonly ILogger<CameraServiceController> _logger;
    private readonly IServiceProvider _serviceprovider;
    private readonly CameraServiceFactory _cameraFactory;

    public CameraServiceController(ILogger<CameraServiceController> logger, IServiceProvider serviceprovider, CameraServiceFactory cameraFactory)
    {
        _logger = logger;
        _serviceprovider = serviceprovider;
        _cameraFactory = cameraFactory;
    }

    [HttpGet("services", Name = "GetServices")]
    public IActionResult GetServices()
    {
        _logger.LogInformation("Getting camera services");
        return Ok(_cameraFactory.GetCameraServices());
    }

    [HttpGet("response/{guid}", Name = "GetResponse")]
    public IActionResult GetResponse(string guid)
    {
        var service = _cameraFactory.GetCameraService(guid);
        if (service == null)
        {
            _logger.LogWarning($"Could not find camera service {guid}");
            return NotFound();
        }
        else
        {
            _logger.LogInformation($"Found service {guid}, name of camera {service?._camera.Name}");
            if (service?.lastReport == null) return NotFound();

            var lastReport = service?.lastReport!;
            if (lastReport != null) _logger.LogInformation($"Last report for camera {service?._camera.Name} was {lastReport.response?.GeneralFeel}");
            else _logger.LogInformation($"Last report for camera {service?._camera.Name} was null");

            return Ok(new CameraServiceResponse { CameraName = service._camera.Name, CameraId = service.Id, IsAnalyzerEnabled = service._camera.EnableAnalyzer, LastResponse = lastReport == null ? new() : lastReport.response });
        }
    }

    [HttpGet("image/{guid}", Name = "GetImage")]
    public IActionResult GetImage(string guid)
    {
        var service = _cameraFactory.GetCameraService(guid);
        if (service == null)
        {
            _logger.LogWarning($"Could not find camera service {guid}");
            return NotFound();
        }
        else
        {
            _logger.LogInformation($"Found service {guid}, name of camera {service?._camera.Name}");
            if (service?.lastImage == null) return NotFound();
            else return Ok(Convert.ToBase64String(service?.lastImage ?? []));
        }
    }

    [HttpGet("enableanalyzer/{guid}", Name = "EnableAnalyzer")]
    public IActionResult EnableAnalyzer(string guid)
    {
        var service = _cameraFactory.GetCameraService(guid);
        if (service == null)
        {
            _logger.LogWarning($"Could not find camera service {guid}");
            return NotFound();
        }
        else
        {
            _logger.LogInformation($"Found service {guid}, name of camera {service?._camera.Name}, enabling image analyzer");
            service?.EnableAnalyzer();
            return Ok();
        }
    }

    [HttpGet("disableanalyzer/{guid}", Name = "DisableAnalyzer")]
    public IActionResult DisableAnalyzer(string guid)
    {
        var service = _cameraFactory.GetCameraService(guid);
        if (service == null)
        {
            _logger.LogWarning($"Could not find camera service {guid}");
            return NotFound();
        }
        else
        {
            _logger.LogInformation($"Found service {guid}, name of camera {service?._camera.Name}, disabling image analyzer");
            service?.DisableAnalyzer();
            return Ok();
        }
    }
}
