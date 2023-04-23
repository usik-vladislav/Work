// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Serializer.Benchmarks;

Console.WriteLine("Hello, World!");

BenchmarkRunner.Run<SerializeBenchmark>();
BenchmarkRunner.Run<DeserializeBenchmark>();
BenchmarkRunner.Run<DeepCopyBenchmark>();