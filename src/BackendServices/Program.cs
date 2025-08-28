using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        int port = args.Length > 0 ? int.Parse(args[0]) : 6000;

        var listener = new TcpListener(IPAddress.Loopback, port);
        listener.Start();

        Console.WriteLine($"[Service:{port}] Listening on port {port}...");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            _ = HandleClientAsync(client, port);
        }
    }

    private static async Task HandleClientAsync(TcpClient client, int port)
    {
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
                    // client closed cleanly
                    Console.WriteLine($"[Service:{port}] Client closed connection.");
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, read).Trim();
                Console.WriteLine($"[Service:{port}] Received: {message}");

                if (string.Equals(message, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    var goodbye = Encoding.UTF8.GetBytes($"[Service:{port}] Goodbye!\n");
                    await stream.WriteAsync(goodbye, 0, goodbye.Length);
                    break;
                }

                var response = Encoding.UTF8.GetBytes($"[Service:{port}] echo: {message}\n");
                await stream.WriteAsync(response, 0, response.Length);
            }
        }
        finally
        {
            client.Close();
        }
    }
}
