using System.Net;
using LoadBalancer.Core.Interfaces;

namespace LoadBalancer.Core.Strategies;

public class RoundRobinStrategy : ILoadBalanceStrategy
{
    private int index = -1;

    public IPEndPoint SelectBackendService(IReadOnlyList<IPEndPoint> backendServices)
    {
        if(backendServices.Count == 0)
            throw new InvalidOperationException("No backend services available");
        
        var next = Interlocked.Increment(ref index);
        return backendServices[next % backendServices.Count];
    }
}