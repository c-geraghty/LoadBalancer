<h1>Load Balancer</h1>


<h3>How to run:</h3>

1. Start backend services on ports 6000 7000, 6001 7001, 6002 7002 via terminal. The second port is for a server health check. <br> ` dotnet run --project .\src\BackendServices\ -- 6000 7000`
2. Start load balancer <br> ` dotnet run --project .\src\LoadBalancer`
3. I used ncat to open connection to the backend services via the load balancer <br> `ncat localhost 5000`

<h3> Load balancer console </h3>

The load balancer will decide what backend service to route the connection to depending on the chosen balancing strategy.

The load balancer console will show what port it is listening on: 5000

It also runs a periodic health check on the backend services to determine whether they are healthy by making a brief connection a health check specific port

<h3> Backend consoles </h3>

The backend console will show the port it is listening on, health check port, and then received messages once a TCP connection has been established

<h3> Client consoles </h3>

Client console shows a greeting from the backend service that it has been routed to

It then will echo any sent messages

<h3> Load Balancing strategies </h3>

There are currently 2 options - Least Active Connections, and Round Robin

This is set in the Program.cs for the LoadBalancer project

<h3> Future considerations </h3>

The Tcp infrastructure code is not very easily testable, as it depends on concrete instantiations of the TcpClient and TcpListener etc

With additional time, I would create a interface/class wrapper around those that allows them to be mocked and tested.

Also, again because of time constraints there are no integration tests. 
