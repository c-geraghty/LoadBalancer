using System.Globalization;
using LoadBalancer.Core.Interfaces;
using LoadBalancer.Core.Services;
using LoadBalancer.Infrastructure.Tcp;
using NSubstitute;
using NSubstitute.Core;
using Xunit;

namespace LoadBalancer.Core.Tests.Services;

public class HealthCheckServiceTests
{
    [Fact]
    public async Task HealthCheckService_ShouldCallAppropriateHealthCheckOnBackend()
    {
        // Arrange
        var backend1 = new TcpBackend("127.0.0.1", 6000, 7000);
        var backend2 = new TcpBackend("127.0.0.1", 6001, 7001);
        var backend3 = new TcpBackend("127.0.0.1", 6002, 7002);
        var backends = new List<IBackendService> { backend1, backend2, backend3 };
        var healthCheck = Substitute.For<IHealthCheck>();
        var cts = new CancellationTokenSource();
        
        var healthCheckService = new HealthCheckService(backends, healthCheck, TimeSpan.FromMilliseconds(200), cts.Token);
        healthCheck.CheckHealthAsync(Arg.Any<IBackendService>(), cts.Token).Returns(true);
        
        // Act
        healthCheckService.CheckHealthAsync();
        await Task.Delay(150);
        cts.Cancel();
        
        // Assert
        healthCheck.Received(3).CheckHealthAsync(Arg.Any<IBackendService>(),Arg.Any<CancellationToken>());
    }
}