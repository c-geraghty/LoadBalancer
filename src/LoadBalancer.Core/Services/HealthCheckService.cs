using LoadBalancer.Core.Interfaces;

namespace LoadBalancer.Core.Services;

public class HealthCheckService
{
    private readonly IEnumerable<IBackendService> _backendServices;
    private readonly IHealthCheck _healthCheck;
    private readonly TimeSpan _checkInterval;
    private readonly CancellationToken _cancellationToken;

    public HealthCheckService(IEnumerable<IBackendService> backendServices, IHealthCheck healthCheck,
        TimeSpan checkInterval, CancellationToken cancellationToken = default)
    {
        _backendServices = backendServices;
        _healthCheck = healthCheck;
        _checkInterval = checkInterval;
        _cancellationToken = cancellationToken;
    }

    public async Task CheckHealthAsync()
    {
        try
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                foreach (var backendService in _backendServices)
                {
                    backendService.IsHealthy = await _healthCheck.CheckHealthAsync(backendService, _cancellationToken);
                    Console.WriteLine($"[HC] Backend {backendService.Id} - Healthy: {backendService.IsHealthy}");
                }
                await Task.Delay(_checkInterval, _cancellationToken);
            }
        }
        catch (OperationCanceledException) when (_cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("[HC] Health check service stopped.");
        }
    }
}