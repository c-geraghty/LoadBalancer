namespace LoadBalancer.Core.Interfaces;

public interface IConnectionHandler 
{
    Task HandleAsync(Stream clientStream, IBackendService target, CancellationToken cancellationToken = default);
}