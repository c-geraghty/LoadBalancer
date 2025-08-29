using System.Net;
using System.Net.Sockets;
using LoadBalancer.Core.Interfaces;

namespace LoadBalancer.Infrastructure.Tcp;

public class TcpConnectionHandler : IConnectionHandler
{
    public async Task HandleAsync(Stream clientStream, IBackendService backendService, CancellationToken cancellationToken = default)
    {
        var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(backendService.Endpoint.Address, backendService.Endpoint.Port);
        
        var targetStream = tcpClient.GetStream();

        backendService.ActiveConnections++;
        Console.WriteLine($"[CH] {backendService.Id} has {backendService.ActiveConnections} active connections");
        
        var t1 = clientStream.CopyToAsync(targetStream, cancellationToken);
        var t2 = targetStream.CopyToAsync(clientStream, cancellationToken);

        // Await either side closing
        await Task.WhenAny(t1, t2);
        backendService.ActiveConnections--;
        Console.WriteLine($"[CH] {backendService.Id} has {backendService.ActiveConnections} active connections");
    }
}