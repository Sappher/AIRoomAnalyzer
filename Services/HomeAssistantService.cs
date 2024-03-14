using HADotNet.Core.Clients;

public class HomeAssistantService
{
    private readonly ILogger<HomeAssistantService> _logger;
    private readonly IServiceProvider _serviceprovider;

    protected ServiceClient? _serviceClient;

    public HomeAssistantService(ILogger<HomeAssistantService> logger, IServiceProvider serviceprovider)
    {
        _logger = logger;
        _serviceprovider = serviceprovider;

        _serviceClient = serviceprovider.GetService<ServiceClient>();
    }

    public async Task<bool> Report(string entity_id, ImageAnalyzeReport report)
    {
        if (_serviceClient == null)
        {
            _logger.LogError("ServiceClient is null, cannot report to Home Assistant");
            return false;
        }
        await _serviceClient.CallService("input_text.set_value", new { entity_id, value = $"General feel at {DateTime.Now}: {report?.response?.GeneralFeel ?? "Error in report"}" });
        _logger.LogInformation($"Reported to Home Assistant: {entity_id} - {report?.response?.GeneralFeel}");
        return true;
    }
}