using System.Net.Sockets;
using LoadBalancer.Core.Interfaces;

namespace LoadBalancer.Infrastructure.Tcp;

public class TcpHealthCheck : IHealthCheck
{
    public async Task<bool> CheckHealthAsync(IBackendService backend, CancellationToken ct)
    {
        if (backend is not TcpBackend tcpBackend)
            throw new ArgumentException("Backend must be TcpBackend", nameof(backend));

        using var client = new TcpClient();
        try 
        {
            await client.ConnectAsync(tcpBackend.Host, tcpBackend.HealthCheckPort).WaitAsync(TimeSpan.FromSeconds(1), ct);
            return client.Connected;
        }
        catch (TimeoutException)
        {
            return false;
        }
        catch (SocketException)
        {
            return false;
        }
    }
}