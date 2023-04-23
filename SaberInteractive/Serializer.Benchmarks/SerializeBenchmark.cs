using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using SerializerTests.Implementations;
using SerializerTests.Nodes;

namespace Serializer.Benchmarks;

[SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
public class SerializeBenchmark
{
    private ListNode _shortData;
    private ListNode _longData;
    private ListNode _veryLongData;

    [GlobalSetup]
    public void Setup()
    {
        _shortData = TestData.ShortListNode();
        _longData = TestData.LongListNode();
        _veryLongData = TestData.VeryLongListNode();
    }

    [Benchmark]
    public async Task ShortBinarySerialize()
    {
        var serializer = new BinarySerializer();
        var stream = new MemoryStream();
        await serializer.Serialize(_shortData, stream);
    }

    [Benchmark]
    public async Task LongBinarySerialize()
    {
        var serializer = new BinarySerializer();
        var stream = new MemoryStream();
        await serializer.Serialize(_longData, stream);
    }

    [Benchmark]
    public async Task VeryLongBinarySerialize()
    {
        var serializer = new BinarySerializer();
        var stream = new MemoryStream();
        await serializer.Serialize(_veryLongData, stream);
    }
}