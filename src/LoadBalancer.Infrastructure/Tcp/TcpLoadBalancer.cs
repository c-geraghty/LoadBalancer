using System.Net;
using System.Net.Sockets;
using LoadBalancer.Core.Interfaces;

namespace LoadBalancer.Infrastructure.Tcp;

public class TcpLoadBalancer : ILoadBalancer
{
    private readonly IPEndPoint _endpoint;
    private readonly List<IBackendService> _backendServices;
    private readonly ILoadBalanceStrategy _strategy;
    private readonly IConnectionHandler _handler;
    private readonly CancellationToken _cancellationToken;

    public TcpLoadBalancer(IPEndPoint endpoint, List<IBackendService> services, ILoadBalanceStrategy strategy,
        IConnectionHandler handler, CancellationToken cancellationToken)
    {
        _endpoint = endpoint;
        _backendServices = services;
        _strategy = strategy;
        _handler = handler;
        _cancellationToken = cancellationToken;
    }
    
    public async Task StartAsync()
    {
        var listener = new TcpListener(_endpoint);
        listener.Start();
        Console.WriteLine($"TCP Load Balancer started on port {_endpoint.Port}. Press ENTER to stop.");
        Console.WriteLine($"Load balancer listening on {_endpoint.Address}:{_endpoint.Port}");

        try
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync(_cancellationToken);
                Console.WriteLine("Requested to LB received");
                _ = Task.Run(() => HandleClientAsync(client), _cancellationToken);
            }
        }
        catch (OperationCanceledException) when (_cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("[LB] Load balancer stopped.");
        }
    }

    private async Task HandleClientAsync(TcpClient client)
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
            await _handler.HandleAsync(clientStream, selectedBackend, _cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LB] Connection handling error: {ex.Message}");
        }
    }
}