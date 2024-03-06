public class CameraServiceResponse
{
    public string CameraName { get; set; } = "";
    public string CameraId { get; set; } = "";
    public ImageAnalyzeReport LastReport { get; set; } = new();
}