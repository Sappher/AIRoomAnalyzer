public interface ICameraService
{
    Task ConnectToCamera(CancellationToken stoppingToken);
}