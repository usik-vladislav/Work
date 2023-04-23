using SerializerTests.Nodes;

namespace Serializer.Benchmarks;

public static class TestData
{
    public static ListNode ShortListNode()
    {
        return Generate(10);
    }

    public static ListNode LongListNode()
    {
        return Generate(1000);
    }

    public static ListNode VeryLongListNode()
    {
        return Generate(100000);
    }

    private static ListNode Generate(int length)
    {
        var dummyHead = new ListNode();
        var previous = dummyHead;

        for (var i = 0; i < length; i++)
        {
            var current = new ListNode
            {
                Data = i.ToString(),
                Previous = previous,
                Random = previous
            };

            previous.Next = current;
            previous = current;
        }

        dummyHead.Next.Previous = null;
        dummyHead.Next.Random = null;

        return dummyHead.Next;
    }
}