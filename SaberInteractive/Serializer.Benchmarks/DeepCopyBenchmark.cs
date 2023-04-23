using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using SerializerTests.Implementations;
using SerializerTests.Nodes;

namespace Serializer.Benchmarks;

[SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
public class DeepCopyBenchmark
{
    private ListNode _shortData;
    private ListNode _longData;
    private ListNode _veryLongData;
    private MemoryStream _stream;

    [GlobalSetup]
    public void Setup()
    {
        _shortData = TestData.ShortListNode();
        _longData = TestData.LongListNode();
        _veryLongData = TestData.VeryLongListNode();
        _stream = new MemoryStream();
    }

    [Benchmark]
    public async Task ShortBinaryDeepCopy()
    {
        var serializer = new BinarySerializer();
        var _ = await serializer.DeepCopy(_shortData);
    }

    [Benchmark]
    public async Task LongBinaryDeepCopy()
    {
        var serializer = new BinarySerializer();
        var _ = await serializer.DeepCopy(_longData);
    }

    [Benchmark]
    public async Task VeryLongBinaryDeepCopy()
    {
        var serializer = new BinarySerializer();
        var _ = await serializer.DeepCopy(_veryLongData);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _stream.Dispose();
    }
}