using System.Net;
using LoadBalancer.Core.Interfaces;
using LoadBalancer.Core.Services;
using LoadBalancer.Core.Strategies;
using LoadBalancer.Infrastructure.Tcp;

Console.WriteLine("Starting LoadBalancer.Api...");

var backends = new List<IBackendService>
{
    new TcpBackend("127.0.0.1", 6000, 7000),
    new TcpBackend("127.0.0.1", 6001, 7001),
    new TcpBackend("127.0.0.1", 6002, 7002)
};

var healthCheck = new TcpHealthCheck();
using var cts = new CancellationTokenSource();

var healthService = new HealthCheckService(
    backends,
    healthCheck,
    TimeSpan.FromSeconds(3),
    cts.Token);

_ = healthService.CheckHealthAsync();

var strategy = new LeastActiveConnectionsStrategy();
var handler = new TcpConnectionHandler();

var listen = new IPEndPoint(IPAddress.Any, 5000);
var balancer = new TcpLoadBalancer(listen, backends, strategy, handler);

var runTask = balancer.StartAsync(cts.Token);

Console.WriteLine("TCP Load Balancer started on port 5000. Press ENTER to stop.");
Console.ReadLine();
cts.Cancel();

await runTask;