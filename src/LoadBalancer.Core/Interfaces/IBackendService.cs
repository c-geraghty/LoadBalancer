using System.Net;

namespace LoadBalancer.Core.Interfaces;

public interface IBackendService
{
    string Id { get; }
    IPEndPoint Endpoint { get; }
    bool IsHealthy { get; set; }
}