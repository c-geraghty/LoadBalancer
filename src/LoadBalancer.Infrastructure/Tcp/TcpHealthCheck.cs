using System.Net.Sockets;
using LoadBalancer.Core.Interfaces;

namespace LoadBalancer.Infrastructure.Tcp;

public class TcpHealthCheck : IHealthCheck
{
    public async Task<bool> CheckHealthAsync(IBackendService backend, CancellationToken ct)
    {
        if (backend is not TcpBackend tcpBackend)
            throw new ArgumentException("Backend must be TcpBackend", nameof(backend));

        try
        {
            using var client = new TcpClient();
            var connectTask = client.ConnectAsync(tcpBackend.Host, tcpBackend.HealthCheckPort);
            var timeoutTask = Task.Delay(1000, ct);

            var completed = await Task.WhenAny(connectTask, timeoutTask);
            return completed == connectTask && client.Connected;
        }
        catch
        {
            return false;
        }
    }
}