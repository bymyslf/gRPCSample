using System;
using gRPCSample.Proto;
using Grpc.Core;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections;

namespace Client
{
    class Program
    {
        private static readonly ChannelsCache channelsCache = new ChannelsCache(
            (port) => (prt) => new Channel($"127.0.0.1:{prt}", ChannelCredentials.Insecure)
        );

        public static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();

        private static async Task MainAsync(string[] args)
        {
            int numServers = 10;
            int.TryParse(args[0], out numServers);

            int numTasks = 10;
            int.TryParse(args[1], out numTasks);

            Console.WriteLine("Spawning server processes...\n");
            var serverProcesses = StartServerProcesses(numServers);

            Console.WriteLine("Spawning worker tasks...\n");
            var workerTasks = StartWorkerTasks(numTasks, serverProcesses.Select(it => it.port).ToArray());

            await Task.WhenAll(workerTasks);
            await Task.WhenAll(channelsCache.Select(ch => ch.ShutdownAsync()));

            foreach (var proc in serverProcesses)
                proc.process.Close();

            Console.WriteLine("Press Ctrl+C to exit...");
            Console.ReadKey();
        }

        private static async Task DoWork((int port, int taskId) data)
        {
            var channel = channelsCache.GetOrAdd(data.port);
            await channel.ConnectAsync(deadline: DateTime.UtcNow.AddSeconds(20));

            var client = new HelloService.HelloServiceClient(channel);
            var reply = await client.SayHelloAsync(new HelloRequest { Name = $"Luís-{data}" }, deadline: DateTime.UtcNow.AddSeconds(20));
            Console.WriteLine($"[Task:{data.taskId}] - Greeting: {reply.Message}");
        }

        private static (Process process, int port)[] StartServerProcesses(int numServers)
        {
            bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            var serverProcesses = new(Process, int)[numServers];
            for (int i = 0, startPort = 50051; i < numServers; i++, startPort++)
            {
                string path = isLinux 
                    ? @"../Server/Server" 
                    : @"..\Server\bin\Debug\netcoreapp2.0\win-x64\publish\Server.exe";
                try
                {
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

        private static Task[] StartWorkerTasks(int numTasks, int[] processPorts)
        {
            int numServers = processPorts.Length;
            var workerTasks = new Task[numTasks];
            for (int i = 0, taskId = 1; i < numTasks; i++, taskId++)
            {
                for (int j = 0; j < numServers; j++)
                {
                    int portTemp = j, taskTemp = taskId;
                    workerTasks[i] = Task.Run(() => DoWork((processPorts[portTemp], taskTemp)));
                }
            }

            return workerTasks;
        }

        private struct ChannelsCache : IEnumerable<Channel>
        {
            private readonly ConcurrentDictionary<int, Lazy<Func<int, Channel>>> dictionary;
            private readonly Func<int, Lazy<Func<int, Channel>>> valueFactory;

            public ChannelsCache(Func<int, Func<int, Channel>> valueFactory)
            {
                dictionary = new ConcurrentDictionary<int, Lazy<Func<int, Channel>>>();
                this.valueFactory = key => new Lazy<Func<int, Channel>>(() => valueFactory(key));
            }

            public Channel GetOrAdd(int key) => dictionary.GetOrAdd(key, valueFactory).Value(key);

            public IEnumerator<Channel> GetEnumerator()
            {
                foreach (var key in dictionary.Keys)
                    yield return dictionary[key].Value(key);
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
