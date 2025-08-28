using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: Must supply port and health check port as args");
            return;
        }
        int port = int.Parse(args[0]);
        int healthCheckPort = int.Parse(args[1]);

        var listener = new TcpListener(IPAddress.Loopback, port);
        listener.Start();
        
        var healthCheckListener = new TcpListener(IPAddress.Loopback, healthCheckPort);
        healthCheckListener.Start();

        Console.WriteLine($"[Service:{port}] Listening on port {port}...");
        Console.WriteLine($"[Service:{port}] HealthCheck listening on port {healthCheckPort}...");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            _ = HandleClientAsync(client, port);
            
            var healthCheckClient = await healthCheckListener.AcceptTcpClientAsync();
            _ = HandleHealthCheckClientAsync(healthCheckClient, healthCheckPort);
        }
    }

    private static async Task HandleHealthCheckClientAsync(TcpClient client, int port)
    {
        using var stream = client.GetStream();
        var buffer = Encoding.UTF8.GetBytes($"[{port}] Health check...\n");
        await stream.WriteAsync(buffer, 0, buffer.Length);
        client.Close(); 
    }

    private static async Task HandleClientAsync(TcpClient client, int port)
    {
        Console.WriteLine($"[Service:{port}] Connection received!");
        
        try
        {
            using var stream = client.GetStream();
            var buffer = new byte[1024];

            var hello = Encoding.UTF8.GetBytes($"[Service:{port}] Hello!\n");
            await stream.WriteAsync(hello, 0, hello.Length);

            while (true)
            {
                int read;
                try
                {
                    read = await stream.ReadAsync(buffer, 0, buffer.Length);
                }
                catch (IOException)
                {
                    Console.WriteLine($"[Service:{port}] Client connection dropped abruptly.");
                    break;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"[Service:{port}] Socket error: {ex.Message}");
                    break;
                }
                
                if (read == 0)
                {
                    Console.WriteLine($"[Service:{port}] Client closed connection.");
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, read).Trim();
                
                if (string.Equals(message, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    var goodbye = Encoding.UTF8.GetBytes($"[Service:{port}] Goodbye!\n");
                    await stream.WriteAsync(goodbye, 0, goodbye.Length);
                    break;
                }
                
                Console.WriteLine($"[Service:{port}] Received: {message}");

                var response = Encoding.UTF8.GetBytes($"[Service:{port}] echo: {message}\n");
                await stream.WriteAsync(response, 0, response.Length);
            }
        }
        finally
        {
            client.Close();
            Console.WriteLine($"[Service:{port}] Connection handler finished - client closed.");
        }
    }
}
