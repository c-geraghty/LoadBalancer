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
            var connectTask = client.ConnectAsync(tcpBackend.Host, tcpBackend.Port);
            var success = connectTask.IsCompleted && client.Connected;
            client.Close();
            return success;
        }
        catch
        {
            return false;
        }
    }
}