using System;
using gRPCSample.Proto;
using Grpc.Core;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        public static void Main(string[] args) => AsyncMain(args).GetAwaiter().GetResult();

        static async Task AsyncMain(string[] args)
        {
            int numServers = 10;
            int.TryParse(args[0], out numServers);

            int numThreads = 10;
            int.TryParse(args[1], out numThreads);

            var ports = Enumerable.Range(50051, numServers);

            Console.WriteLine("Spawning worker threads...\n");
            var workerThreads = StartWorkerThreads(numThreads, ports.ToArray());

            Console.WriteLine("Spawning server processes...\n");
            var serverProcesses = StartServerProcesses(numServers);

            //Console.WriteLine("Spawning worker threads...\n");
            //var workerThreads = StartWorkerThreads(numThreads, serverProcesses.Select(it => it.port).ToArray());

            foreach (var thread in workerThreads)
                thread.Join(500);

            foreach (var proc in serverProcesses)
            {
                proc.process.Close();
                //proc.process.Kill();
            }
                
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void DoWork((int port, int threadId) data)
        {
            Channel channel = new Channel($"127.0.0.1:{data.port}", ChannelCredentials.Insecure);
            channel.ConnectAsync(deadline: DateTime.UtcNow.AddSeconds(20)).Wait();

            var client = new HelloService.HelloServiceClient(channel);
           
            String user = $"Luís-{data}";

            var reply = client.SayHello(new HelloRequest { Name = user }, deadline: DateTime.UtcNow.AddSeconds(20));
            Console.WriteLine($"[Thread:{data.threadId}] - Greeting: {reply.Message}");

            channel.ShutdownAsync().Wait();
        }

        private static (Process process, int port)[] StartServerProcesses(int numServers)
        {
            bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            var serverProcesses = new(Process, int)[numServers];
            for (int i = 0, startPort = 50051; i < numServers; i++, startPort++)
            {
                string path = isLinux 
                    ? @"../Server/Server" 
                    : @"D:\GitProjects\gRPCSample\Server\bin\Debug\netcoreapp2.0\win-x64\publish\Server.exe";
                try
                {
                    Console.WriteLine(path);
                    serverProcesses[i] = (Process.Start(path, $"{startPort}"), startPort);
                }
                catch (Win32Exception w)
                {
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

            return serverProcesses;
        }

        private static Thread[] StartWorkerThreads(int numThreads, int[] processPorts)
        {
            int numServers = processPorts.Length;
            var workerThreads = new Thread[numThreads];
            for (int i = 0, threadId = 0; i < numThreads; i++, threadId++)
            {
                for (int j = 0; j < numServers; j++)
                {
                    int portTemp = j;
                    workerThreads[i] = new Thread(() => DoWork((processPorts[portTemp], threadId)));
                    workerThreads[i].Start();
                }
            }

            return workerThreads;
        }
    }
}
