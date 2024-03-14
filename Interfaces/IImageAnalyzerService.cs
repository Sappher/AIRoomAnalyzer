public class ImageAnalyzeReport
{
    public byte[]? image;
    public bool? error;
    public AIRoomResponse? response;
}

public interface IImageAnalyzerService
{
    public Task<ImageAnalyzeReport> AnalyzeImage(byte[] data, CancellationToken? cancellationToken);
}