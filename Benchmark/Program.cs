﻿using System;
using BenchmarkDotNet.Running;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Benchmark>();

            Console.WriteLine("Benchmark completed");
            Console.ReadLine();
        }
    }
}
