using System;
using System.Threading.Tasks;
using Core = Grpc.Core;
using gRPCSample.Proto;

namespace Server
{
    class HelloServiceImpl : HelloService.HelloServiceBase
    {
        private readonly int servicePort;

        public HelloServiceImpl(int servicePort)
        {
            this.servicePort = servicePort;
        }

        public override Task<HelloResponse> SayHello(HelloRequest request, Core.ServerCallContext context)
        {
            return Task.FromResult(new HelloResponse { Message = $"Hello {request.Name}"});
        }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                throw new InvalidOperationException("Port should be defined");
            }

            int.TryParse(args[0], out int port);

            Core.Server server = new Core.Server
            {
                Services = { HelloService.BindService(new HelloServiceImpl(port)) },
                Ports = { new Core.ServerPort("localhost", port, Core.ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("HelloService server listening on port " + port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
