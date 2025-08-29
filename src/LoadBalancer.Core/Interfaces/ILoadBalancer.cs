namespace LoadBalancer.Core.Interfaces;

public interface ILoadBalancer<T> where T : IBackendService
{
    Task StartAsync();
}