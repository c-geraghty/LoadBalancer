using System.Net;

namespace LoadBalancer.Core.Interfaces;

public interface ILoadBalanceStrategy
{
    IBackendService SelectBackendService(IReadOnlyList<IBackendService> backendServices);
}