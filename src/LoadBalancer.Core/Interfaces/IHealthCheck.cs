namespace LoadBalancer.Core.Interfaces;

public interface IHealthCheck
{
    Task<bool> CheckHealthAsync(IBackendService backend, CancellationToken ct);
}