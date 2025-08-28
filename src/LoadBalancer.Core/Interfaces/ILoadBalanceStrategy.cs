using System.Net;

namespace LoadBalancer.Core.Interfaces;

public interface ILoadBalanceStrategy
{
    IPEndPoint SelectBackendService(IReadOnlyList<IPEndPoint> backendServices);
}