using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using SerializerTests.Implementations;
using SerializerTests.Nodes;

namespace Serializer.Benchmarks;

[SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
public class DeserializeBenchmark
{
    private byte[] _shortData;
    private byte[] _longData;
    private byte[] _veryLongData;

    [GlobalSetup]
    public void Setup()
    {
        _shortData = GetStream(TestData.ShortListNode());
        _longData = GetStream(TestData.LongListNode());
        _veryLongData = GetStream(TestData.VeryLongListNode());
    }

    [Benchmark]
    public async Task ShortBinaryDeserialize()
    {
        var serializer = new BinarySerializer();
        var stream = new MemoryStream(_shortData);
        var _ = await serializer.Deserialize(stream);
    }

    [Benchmark]
    public async Task LongBinaryDeserialize()
    {
        var serializer = new BinarySerializer();
        var stream = new MemoryStream(_longData);
        var _ = await serializer.Deserialize(stream);
    }

    [Benchmark]
    public async Task VeryLongBinaryDeserialize()
    {
        var serializer = new BinarySerializer();
        var stream = new MemoryStream(_veryLongData);
        var _ = await serializer.Deserialize(stream);
    }

    private static byte[] GetStream(ListNode node)
    {
        using var stream = new MemoryStream();
        var serializer = new BinarySerializer();
        serializer.Serialize(node, stream);

        return stream.ToArray();
    }
}