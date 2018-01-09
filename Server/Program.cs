using System;
using System.Threading.Tasks;
using Core = Grpc.Core;
using gRPCSample.Proto;
using System.Threading;

namespace Server
{
    class HelloServiceImpl : HelloService.HelloServiceBase
    {
        private readonly int servicePort;

        public HelloServiceImpl(int servicePort)
            => this.servicePort = servicePort;

        public override Task<HelloResponse> SayHello(HelloRequest request, Core.ServerCallContext context)
            => Task.FromResult(new HelloResponse { Message = $"Hello {request.Name}" });
    }

    class Program
    {
        private static ManualResetEvent quiteEvent = new ManualResetEvent(false);

        public static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();

        private static async Task MainAsync(string[] args)
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                quiteEvent.Set();
                eventArgs.Cancel = true;
            };

            int port = 50051;
            int.TryParse(args[0], out port);

            Core.Server server = new Core.Server
            {
                Services = { HelloService.BindService(new HelloServiceImpl(port)) },
                Ports = { new Core.ServerPort("localhost", port, Core.ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("HelloService server listening on port " + port);

            quiteEvent.WaitOne();

            await server.ShutdownAsync();
        }
    }
}
