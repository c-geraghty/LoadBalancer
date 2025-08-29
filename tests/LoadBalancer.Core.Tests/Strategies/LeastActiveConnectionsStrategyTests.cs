using LoadBalancer.Core.Interfaces;
using LoadBalancer.Core.Strategies;
using LoadBalancer.Infrastructure.Tcp;
using Xunit;
using Assert = NUnit.Framework.Assert;

namespace LoadBalancer.Core.Tests.Strategies;

public class LeastActiveConnectionsStrategyTests
{
    [Fact]
    public void LeastActiveConnectionsStrategy_ShouldChooseBackendWithLeastConnections()
    {
        // Arrange
        var backend1 = new TcpBackend("127.0.0.1", 6000, 7000);
        var backend2 = new TcpBackend("127.0.0.1", 6001, 7001);

        backend1.ActiveConnections = 10;
        backend2.ActiveConnections = 2;
        
        var backends = new List<IBackendService> { backend1, backend2 };
        var rr = new LeastActiveConnectionsStrategy();
        
        // Act
        var first = rr.SelectBackendService(backends);
        var second = rr.SelectBackendService(backends);

        // Assert
        Assert.AreEqual(first.Endpoint.Port, backend2.Endpoint.Port);
        Assert.AreEqual(second.Endpoint.Port, backend2.Endpoint.Port);
    }
}