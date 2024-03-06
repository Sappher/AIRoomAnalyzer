using OpenAI.ObjectModels.RequestModels;

public interface IPromptsService
{
    public List<ChatMessage> FixedPromps { get; set; }
    public List<ChatMessage> AdditionalPrompts { get; set; }
    public List<ChatMessage> AllPrompts { get => FixedPromps.Concat(AdditionalPrompts).ToList(); }
}