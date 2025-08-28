using System.Net;
using System.Net.Sockets;
using LoadBalancer.Core.Interfaces;

namespace LoadBalancer.Infrastructure.Tcp;

public class TcpLoadBalancer : ILoadBalancer<IBackendService>
{
    private readonly IPEndPoint _endpoint;
    private readonly List<IBackendService> _backendServices;
    private readonly ILoadBalanceStrategy _strategy;
    private readonly IConnectionHandler _handler;

    public TcpLoadBalancer(IPEndPoint endpoint, List<IBackendService> services, ILoadBalanceStrategy strategy,
        IConnectionHandler handler)
    {
        _endpoint = endpoint;
        _backendServices = services;
        _strategy = strategy;
        _handler = handler;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var listener = new TcpListener(_endpoint);
        listener.Start();
        Console.WriteLine($"Load balancer listening on {_endpoint.Address}:{_endpoint.Port}");

        while (!cancellationToken.IsCancellationRequested)
        {
            var client = await listener.AcceptTcpClientAsync(cancellationToken);
            Console.WriteLine("Requested to LB received");
            _ = Task.Run(() => HandleClientAsync(client, cancellationToken), cancellationToken);
        }
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
        var clientStream = client.GetStream();
        
        var healthyServices = _backendServices.Where(t => t.IsHealthy).ToList();
        if (!healthyServices.Any())
        {
            Console.WriteLine($"[LB] No healthy backend services");
            client.Close();
            return; 
        }

        var selectedBackend = _strategy.SelectBackendService(healthyServices);
        Console.WriteLine($"[LB] Selected Backend Target: {selectedBackend.Id} -> {selectedBackend.Endpoint}");
        
        try
        {
            await _handler.HandleAsync(clientStream, selectedBackend, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LB] Connection handling error: {ex.Message}");
        }
    }
}