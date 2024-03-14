using System.Diagnostics;
using System.Net.NetworkInformation;
using CameraService.Configuration;
using HADotNet.Core.Clients;
using Helpers;

namespace CameraService
{
    public class CameraService : BackgroundService
    {
        public string Guid;
        protected bool _connected = false;

        private readonly ILogger<CameraService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IImageAnalyzerService _imageAnalyzerService;
        public readonly CameraServiceConfiguration _camera;

        public byte[]? lastImage;
        public ImageAnalyzeReport? lastReport;

        public CameraService(string guid, ILogger<CameraService> logger, IServiceProvider serviceProvider, IImageAnalyzerService imageAnalyzerService, CameraServiceConfiguration camera)
        {
            Guid = guid;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _imageAnalyzerService = imageAnalyzerService;
            _camera = camera;

            _logger.LogInformation("CameraService created for camera: " + _camera.Name);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CameraService started for camera: " + _camera.Name);
            await ExecuteAsync(cancellationToken);
        }

        public void EnableAnalyzer()
        {
            _camera.EnableAnalyzer = true;
        }

        public void DisableAnalyzer()
        {
            _camera.EnableAnalyzer = false;
        }

        public async Task ConnectToCamera(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation($"Adding camera: {_camera.Name} from {_camera.Url}");

                var frameCaptureInterval = _camera.FrameCaptureInterval != null ? _camera.FrameCaptureInterval : 20;
                if (frameCaptureInterval < 5)
                {
                    _logger.LogWarning($"Frame capture interval for camera {_camera.Name} is too low, setting it to 5");
                    frameCaptureInterval = 5;
                }

                // Extract the hostname or IP address from the URL
                var uri = new Uri(_camera.Url);
                var host = uri.Host;

                // Ping the camera to see if it's online
                var ping = new Ping();
                var reply = await ping.SendPingAsync(host);

                if (reply.Status != IPStatus.Success)
                {
                    _logger.LogError($"Camera {_camera.Name} is not online, skipping");
                    return;
                }
                else
                {
                    _logger.LogInformation($"Camera {_camera.Name} is online, starting ffmpeg process");
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    //Arguments = $"-y -i {camera.Url} -frames:v 1 -f image2pipe -vcodec png -", // Single png file
                    Arguments = $"-y {_camera.ExtraFFMpegInputArguments} -i {_camera.Url} -r 1/{frameCaptureInterval} {_camera.ExtraFFMpegOutputArguments} -f image2pipe -vcodec png -", // Continuous png stream
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = _camera.ShowFFMpegOutput == true ? false : true,
                    CreateNoWindow = true
                };

                using var process = Process.Start(startInfo) ?? throw new Exception("Failed to start ffmpeg process");

                _logger.LogInformation($"ffmpeg process started for camera {_camera.Name} with arguments: {startInfo.Arguments}");

                using (var output = process.StandardOutput.BaseStream)
                {
                    var buffer = new byte[16 * 1024];
                    using (var ms = new MemoryStream())
                    {
                        int read;
                        bool isPng = false;
                        while ((read = await output.ReadAsync(buffer, stoppingToken)) > 0)
                        {
                            ms.Write(buffer, 0, read); // write the buffer to memory stream
                            if (isPng == false && ByteFuntions.IsPngImage(ms)) // check if the data is PNG image (should be)
                                isPng = true;
                            if (isPng && ByteFuntions.IsPngEndChunk(ms)) // check whether end of PNG image file has been reached
                            {
                                _logger.LogInformation("PNG image detected");
                                var data = ms.ToArray();
                                _logger.LogInformation($"Whole PNG received, length {data.Length} bytes from camera {_camera.Name}");
                                // reset the memory stream
                                ms.Position = 0;
                                ms.SetLength(0);

                                if (_camera.EnableAnalyzer == true && stoppingToken.IsCancellationRequested == false)
                                {
                                    _logger.LogInformation($"Analyzing image of camera {_camera.Name}");
                                    // Send the image to be analyzed. Don't wait for it to execute, continue executing
                                    _ = Task.Run(() => _imageAnalyzerService.AnalyzeImage(data, stoppingToken).ContinueWith((resp) => { lastReport = resp.Result; lastImage = resp.Result.image; }), stoppingToken);
                                }
                                else lastReport = new ImageAnalyzeReport() { image = data, error = false, response = new AIRoomResponse() { } };
                            }

                            if (stoppingToken.IsCancellationRequested)
                                break;
                        }
                    }
                }

                process.WaitForExit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error connecting to camera {_camera.Name}");
                throw;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ConnectToCamera(stoppingToken);
                    break;
                }
                catch (Exception ex)
                {
                    if (!stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogError($"Error handling camera connection of {_camera.Name}: {ex.Message}, trying again in 5 seconds...");
                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // Wait for 5 seconds before retrying
                    }
                }
            }
        }
    }
}
