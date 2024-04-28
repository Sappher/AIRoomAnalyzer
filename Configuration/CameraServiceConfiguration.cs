
namespace CameraService.Configuration
{
    public class CameraServiceConfiguration
    {
        public bool EnableAnalyzer { get; set; } = false;
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Room { get; set; } = "";
        public string Url { get; set; } = "";
        public string? SubUrl { get; set; }
        public int? FrameCaptureInterval { get; set; }
        public string? ExtraFFMpegInputArguments { get; set; }
        public string? ExtraFFMpegOutputArguments { get; set; }
        public bool ShowFFMpegOutput { get; set; } = false;
    }
}
