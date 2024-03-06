public class HomeAssistantService : IHomeAssistantService
{
    private readonly ILogger<HomeAssistantService> _logger;
    private readonly IServiceProvider _serviceprovider;

    public HomeAssistantService(ILogger<HomeAssistantService> logger, IServiceProvider serviceprovider)
    {
        _logger = logger;
        _serviceprovider = serviceprovider;
    }

    public string TestService()
    {
        return "Test";
    }

    /*public async Task<HttpResponseMessage> GetEntities()
    {
        var ha = _serviceprovider.GetRequiredService<IHomeAssistantService>();
        var entities = await ha.GetEntities();
        return new HttpResponseMessage()
        {
            Content = new StringContent(JsonSerializer.Serialize(entities))
        };
    }

    public async Task<HttpResponseMessage> GetStates()
    {
        var ha = _serviceprovider.GetRequiredService<IHomeAssistantService>();
        var states = await ha.GetStates();
        return new HttpResponseMessage()
        {
            Content = new StringContent(JsonSerializer.Serialize(states))
        };
    }

    public async Task<HttpResponseMessage> GetServices()
    {
        var ha = _serviceprovider.GetRequiredService<IHomeAssistantService>();
        var services = await ha.GetServices();
        return new HttpResponseMessage()
        {
            Content = new StringContent(JsonSerializer.Serialize(services))
        };
    }

    public async Task<HttpResponseMessage> CallService(string domain, string service, string? entityId = null, Dictionary<string, object>? serviceData = null)
    {
        var ha = _serviceprovider.GetRequiredService<IHomeAssistantService>();
        var response = await ha.CallService(domain, service, entityId, serviceData);
        return new HttpResponseMessage()
        {
            Content = new StringContent(JsonSerializer.Serialize(response))
        };
    }*/
}