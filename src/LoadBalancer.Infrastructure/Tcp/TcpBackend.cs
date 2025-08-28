
using System.Net;
using LoadBalancer.Core.Interfaces;

namespace LoadBalancer.Infrastructure.Tcp;

public class TcpBackend : IBackendService
{
    public string Host { get; }
    public int Port { get; }
    public int HealthCheckPort { get; }

    public bool IsHealthy { get; set; } = true;
    public IPEndPoint Endpoint { get; }
    public int ActiveConnections { get; set; } = 0;

    public TcpBackend(string host, int port, int healthCheckPort)
    {
        Host = host;
        Port = port;
        HealthCheckPort = healthCheckPort;
        Endpoint = new IPEndPoint(IPAddress.Parse(host), port);
    }

    public string Id  => $"{Host}:{Port}";
};
