using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using MMDHelpers.CSharp.Extensions;

namespace StressTest
{
    public class EndsWithBenchmark
    {
        [Benchmark]
        public bool newVersion() => @"C:\project\MMDHelpers.CSharp\MMDHelpers.CSharp.Performance.Grpc\bin\Release\MMDHelpers.CSharp.Performance.Grpc.1.0.1.nupkg".EndsWithCaseSensitive( ".nupkg");

        [Benchmark]
        public bool Possible() => @"C:\project\MMDHelpers.CSharp\MMDHelpers.CSharp.Performance.Grpc\bin\Release\MMDHelpers.CSharp.Performance.Grpc.1.0.1.nupkg".EndsWith(".nupkg");

        [Benchmark]
        public bool Current() => Path.GetExtension(@"C:\project\MMDHelpers.CSharp\MMDHelpers.CSharp.Performance.Grpc\bin\Release\MMDHelpers.CSharp.Performance.Grpc.1.0.1.nupkg") == ".nupkg";
        
    }




    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<EndsWithBenchmark>();
        }
    }
}
