using System.Net;
using LoadBalancer.Core.Interfaces;
using LoadBalancer.Core.Services;
using LoadBalancer.Core.Strategies;
using LoadBalancer.Infrastructure.Tcp;

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

var strategy = new LeastActiveConnectionsStrategy();
var handler = new TcpConnectionHandler();

var listen = new IPEndPoint(IPAddress.Any, 5000);
var balancer = new TcpLoadBalancer(listen, backends, strategy, handler, cts.Token);

var balancerTask = balancer.StartAsync();
var healthTask = healthService.CheckHealthAsync(); 

Console.ReadLine();

cts.Cancel(); 

await Task.WhenAll(healthTask, balancerTask);