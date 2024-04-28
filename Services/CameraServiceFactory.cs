using CameraService.Configuration;
using Microsoft.Extensions.Options;

namespace CameraService
{
    public class CameraServiceFactory : BackgroundService
    {
        private readonly ILogger<CameraServiceFactory> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly List<CameraServiceConfiguration> _cameras;

        protected Dictionary<string, CameraService> _cameraServices = new();

        public CameraServiceFactory(ILogger<CameraServiceFactory> logger, IServiceProvider serviceProvider, IOptions<List<CameraServiceConfiguration>> configs)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _cameras = configs.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tasks = new List<Task>();

            foreach (var camera in _cameras)
            {
                _logger.LogInformation($"Adding {camera.Name} service, id {camera.Id}");
                var service = ActivatorUtilities.CreateInstance<CameraService>(_serviceProvider, camera, camera.Id);
                _cameraServices.Add(camera.Id, service);
                tasks.Add(service.StartAsync(stoppingToken));
            }

            await Task.WhenAll(tasks);
        }

        public List<string> GetCameraServices()
        {
            return _cameraServices.Keys.ToList();
        }

        public CameraService? GetCameraService(string guid)
        {
            return _cameraServices[guid];
        }
    }
}