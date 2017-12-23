using BenchmarkDotNet.Attributes;
using Grpc.Core;
using gRPCSample.Proto;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Benchmark
{
    public class Benchmark
    {
        private const int Port = 50051;

        private readonly Channel channel;

        public Benchmark()
        {
            this.channel = new Channel($"127.0.0.1:{Port}", ChannelCredentials.Insecure);
        }

        [GlobalSetup]
        public void GlobalSetup() => this.Setup();

        [GlobalCleanup]
        public void GlobalCleanup() => this.channel.ShutdownAsync().Wait();

        [Benchmark]
        public async Task Run() => await this.RequestServer();

        private void Setup()
        {
            bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            string path = isLinux
                    ? @"../Server/Server"
                    : @"..\Server\bin\Release\netcoreapp2.0\win-x64\publish\Server.exe";
            try
            {
                Process.Start(path, $"{Port}");
            }
            catch (Win32Exception w)
            {
                Console.WriteLine($"Working directory: {Directory.GetCurrentDirectory()}");
                Console.WriteLine($"Message: {w.Message}");
                Console.WriteLine($"ErrorCode: {w.ErrorCode.ToString()}");
                Console.WriteLine($"NativeErrorCode: {w.NativeErrorCode.ToString()}");
                Console.WriteLine($"StackTrace: {w.StackTrace}");
                Console.WriteLine($"Source: {w.Source}");
                Exception e = w.GetBaseException();
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private async Task RequestServer()
        {
            await channel.ConnectAsync(deadline: DateTime.UtcNow.AddSeconds(20));

            var client = new HelloService.HelloServiceClient(channel);
            await client.SayHelloAsync(new HelloRequest { Name = $"Luís" }, deadline: DateTime.UtcNow.AddSeconds(20));
        }
    }
}
