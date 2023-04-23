using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using SerializerTests.Implementations;
using SerializerTests.Interfaces;
using SerializerTests.Nodes;
using Xunit;

namespace Serializer.Tests;

public class ListSerializerTests
{
    private readonly Fixture _fixture;
    private readonly IListSerializer _target;

    public ListSerializerTests()
    {
        _fixture = new Fixture();
        _target = new BinarySerializer();
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public async Task TestSerializeDeserialize(int?[] randomIndexes)
    {
        var node = BuildNodes(randomIndexes);

        await using var stream = new MemoryStream();
        await _target.Serialize(node, stream);

        await using var newStream = new MemoryStream(stream.ToArray());
        var deserializedNode = await _target.Deserialize(newStream);

        AssertDifferentNodes(node, deserializedNode);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public async Task TestDeepCopy(int?[] randomIndexes)
    {
        var node = BuildNodes(randomIndexes);

        var copiedNode = await _target.DeepCopy(node);

        AssertDifferentNodes(node, copiedNode);
    }

    private ListNode BuildNodes(IReadOnlyCollection<int?> randomIndexes)
    {
        if (!randomIndexes.Any())
        {
            return null;
        }

        var indexesMap = new Dictionary<int, ListNode>();
        var dummyHead = new ListNode();
        var previous = dummyHead;

        for (var i = 0; i < randomIndexes.Count; i++)
        {
            var current = new ListNode
            {
                Data = _fixture.Create<string>(),
                Previous = previous
            };

            indexesMap[i] = current;
            previous.Next = current;
            previous = current;
        }

        previous = dummyHead;

        foreach (var randomIndex in randomIndexes)
        {
            if (randomIndex.HasValue)
            {
                previous.Next.Random = indexesMap[randomIndex.Value];
            }

            previous = previous.Next;
        }

        dummyHead.Next.Previous = null;

        return dummyHead.Next;
    }

    private static void AssertDifferentNodes(ListNode leftNode, ListNode rightNode)
    {
        Assert.Equal(leftNode is null, rightNode is null);

        if (leftNode is null)
        {
            return;
        }

        var currentLeftNode = leftNode;
        var currentRightNode = rightNode;
        var nodesToCopyNodesMap = new Dictionary<ListNode, ListNode>();

        while (currentLeftNode is not null && currentRightNode is not null)
        {
            Assert.NotSame(currentLeftNode, currentRightNode);
            Assert.Equal(currentLeftNode.Data, currentRightNode.Data);
            Assert.Equal(currentLeftNode.Previous is null, currentRightNode.Previous is null);
            Assert.Equal(currentLeftNode.Random is null, currentRightNode.Random is null);

            if (currentLeftNode.Previous is not null)
            {
                Assert.Equal(nodesToCopyNodesMap[currentLeftNode.Previous], currentRightNode.Previous);
            }

            nodesToCopyNodesMap[currentLeftNode] = currentRightNode;

            currentLeftNode = currentLeftNode.Next;
            currentRightNode = currentRightNode.Next;
        }

        Assert.Null(currentLeftNode);
        Assert.Null(currentRightNode);

        currentLeftNode = leftNode;
        currentRightNode = rightNode;

        while (currentLeftNode is not null)
        {
            if (currentLeftNode.Random is not null)
            {
                Assert.Equal(nodesToCopyNodesMap[currentLeftNode.Random], currentRightNode.Random);
            }

            currentLeftNode = currentLeftNode.Next;
            currentRightNode = currentRightNode.Next;
        }
    }

    public static IEnumerable<object[]> TestData => new List<object[]>
    {
        new object[] { new int?[] { 2, 0, 3, 1 } },
        new object[] { new int?[] { 0, 1, 2, 3 } },
        new object[] { new int?[] { 3, null, null, 1 } },
        new object[] { new int?[] { null, null } },
        new object[] { new int?[] { } },
        new object[] { new int?[] { 1, 1, 1, 1 } },
    };
}