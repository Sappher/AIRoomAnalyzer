public class CameraServiceResponse
{
    public string CameraName { get; set; } = "";
    public string CameraId { get; set; } = "";
    public bool IsAnalyzerEnabled { get; set; } = false;
    public AIRoomResponse LastResponse { get; set; } = new();
}