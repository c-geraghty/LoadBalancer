using System.Net;
using System.Net.Sockets;
using LoadBalancer.Core.Interfaces;

namespace LoadBalancer.Infrastructure.Tcp;

public class TcpConnectionHandler : IConnectionHandler
{
    public async Task HandleAsync(Stream clientStream, IBackendService target, CancellationToken cancellationToken = default)
    {
        if(target is not IBackendService backendService)
            throw new ArgumentException("Target must be a backend service");

        var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(backendService.Endpoint.Address, backendService.Endpoint.Port);
        
        var targetStream = tcpClient.GetStream();

        target.ActiveConnections++;
        Console.WriteLine($"[CH] {target.Id} has {target.ActiveConnections} active connections");
        
        var t1 = clientStream.CopyToAsync(targetStream, cancellationToken);
        var t2 = targetStream.CopyToAsync(clientStream, cancellationToken);

        // Await either side closing
        await Task.WhenAny(t1, t2);
        target.ActiveConnections--;
        Console.WriteLine($"[CH] {target.Id} has {target.ActiveConnections} active connections");
    }
}