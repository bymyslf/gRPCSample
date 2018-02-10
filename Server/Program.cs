using System;
using System.Threading.Tasks;
using Core = Grpc.Core;
using gRPCSample.Proto;
using Google.Protobuf.WellKnownTypes;

namespace Server
{
    class ShutdownServiceImpl : ShutdownService.ShutdownServiceBase
    {
        private volatile bool _shutdownRequested;
        private readonly TaskCompletionSource<object> _shutdownTcs = new TaskCompletionSource<object>();

        public Task ShutdownTask => _shutdownTcs.Task;

        public override Task<Empty> Shutdown(Empty request, Core.ServerCallContext context)
        {
            lock (_shutdownTcs)
            {
                if (_shutdownRequested)
                    throw new InvalidOperationException("Service shutdown already requested");
 
                _shutdownRequested = true;
            }

            _shutdownTcs.SetResult(null);

            return Task.FromResult(new Empty());
        }
    }

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
        public static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();

        private static async Task MainAsync(string[] args)
        {
            int port = 50051;
            int.TryParse(args[0], out port);

            var shutdownService = new ShutdownServiceImpl();

            Core.Server server = new Core.Server
            {
                Services =
                {
                    HelloService.BindService(new HelloServiceImpl(port)),
                    ShutdownService.BindService(shutdownService)
                },
                Ports = { new Core.ServerPort("localhost", port, Core.ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("HelloService server listening on port " + port);

            await shutdownService.ShutdownTask;
            await server.ShutdownAsync();
        }
    }
}
