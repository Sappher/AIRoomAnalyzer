using OpenAI.ObjectModels.RequestModels;

public class PromptsService : IPromptsService
{
    private readonly ILogger<PromptsService> _logger;
    private readonly IConfiguration _config;
    private readonly IServiceProvider _serviceprovider;

    public List<ChatMessage> FixedPromps { get; set; } = new List<ChatMessage>();
    public List<ChatMessage> AdditionalPrompts { get; set; } = new List<ChatMessage>();

    public PromptsService(ILogger<PromptsService> logger, IConfiguration config, IServiceProvider serviceprovider)
    {
        _logger = logger;
        _config = config;
        _serviceprovider = serviceprovider;

        var prompts = _config.GetSection("FixedPrompts").Get<List<string>>();
        if (prompts != null)
        {
            foreach (var prompt in prompts)
                FixedPromps.Add(ChatMessage.FromSystem(prompt));
        }
        var trackedObjects = _config.GetSection("TrackedObjects").Get<List<string>>();
        if (trackedObjects != null)
        {
            FixedPromps.Add(ChatMessage.FromSystem("Try to find these following objects in the image and if they're found, place them in a JSON array in the response with key trackedObjects: " + string.Join(", ", trackedObjects)));
        }
        else _logger.LogWarning("No fixed prompts found in configuration.");
    }
}