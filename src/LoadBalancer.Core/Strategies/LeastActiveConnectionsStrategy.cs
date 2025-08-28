using System.Net;
using LoadBalancer.Core.Interfaces;

namespace LoadBalancer.Core.Strategies;

public class LeastActiveConnectionsStrategy : ILoadBalanceStrategy
{
    public IBackendService SelectBackendService(IReadOnlyList<IBackendService> backendServices)
    {
        if(backendServices.Count == 0)
            throw new InvalidOperationException("No backend services available");

        var leastConnectedService = backendServices.OrderBy(s => s.ActiveConnections).First();
        return leastConnectedService;
    }
}