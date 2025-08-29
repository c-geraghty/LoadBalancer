using System.Net;
using LoadBalancer.Core.Interfaces;
using LoadBalancer.Core.Strategies;
using LoadBalancer.Infrastructure.Tcp;
using NSubstitute;
using Xunit;
using Assert = NUnit.Framework.Assert;

namespace LoadBalancer.Core.Tests.Strategies;

public class RoundRobinStrategyTests
{
    [Fact]
    public void RoundRobinStrategy_ShouldChooseBackendsInOrder()
    {
        // Arrange
        var backend1 = new TcpBackend("127.0.0.1", 6000, 7000);
        var backend2 = new TcpBackend("127.0.0.1", 6001, 7001);
        var backend3 = new TcpBackend("127.0.0.1", 6002, 7002);
        var backends = new List<IBackendService> { backend1, backend2, backend3 };
        var rr = new RoundRobinStrategy();
        
        // Act
        var first = rr.SelectBackendService(backends);
        var second = rr.SelectBackendService(backends);
        var third = rr.SelectBackendService(backends);

        // Assert
        Assert.AreEqual(first.Endpoint.Port, backend1.Endpoint.Port);
        Assert.AreEqual(second.Endpoint.Port, backend2.Endpoint.Port);
        Assert.AreEqual(third.Endpoint.Port, backend3.Endpoint.Port);
    }
}