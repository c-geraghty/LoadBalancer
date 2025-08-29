namespace LoadBalancer.Core.Interfaces;

public interface IConnectionHandler 
{
    Task HandleAsync(Stream clientStream, IBackendService backendService, CancellationToken cancellationToken = default);
}