using System.Text;
using SerializerTests.Interfaces;
using SerializerTests.Nodes;

namespace SerializerTests.Implementations
{
    //Specify your class\file name and complete implementation.
    public class BinarySerializer : IListSerializer
    {
        //the constructor with no parameters is required and no other constructors can be used.
        public BinarySerializer()
        {
            //...
        }

        public Task<ListNode> DeepCopy(ListNode head)
        {
            if (head is null)
            {
                return Task.FromResult<ListNode>(null);
            }

            var copyNodesMap = new Dictionary<ListNode, ListNode>();
            var dummyHead = new ListNode();
            var previousCopy = dummyHead;
            var current = head;

            while (current is not null)
            {
                var currentCopy = new ListNode
                {
                    Data = current.Data,
                    Previous = previousCopy,
                    Random = current.Random
                };

                copyNodesMap[current] = currentCopy;

                previousCopy.Next = currentCopy;
                previousCopy = currentCopy;
                current = current.Next;
            }

            current = dummyHead.Next;

            while (current is not null)
            {
                current.Random = current.Random is null ? null : copyNodesMap[current.Random];
                current = current.Next;
            }

            dummyHead.Next.Previous = null;

            return Task.FromResult(dummyHead.Next);
        }

        public Task<ListNode> Deserialize(Stream s)
        {
            using var reader = new BinaryReader(s, Encoding.UTF8);
            var length = reader.ReadInt32();

            if (length == 0)
            {
                return Task.FromResult<ListNode>(null);
            }

            var nodeAndRandomIndexByIndexMap = new Dictionary<int, (ListNode Node, int RandomIndex)>(length);
            var dummyHead = new ListNode();
            var previous = dummyHead;

            for (var i = 0; i < length; i++)
            {
                var data = reader.ReadString();
                var randomIndex = reader.ReadInt32();
                var current = new ListNode
                {
                    Data = data,
                    Previous = previous
                };

                nodeAndRandomIndexByIndexMap[i] = (current, randomIndex);

                previous.Next = current;
                previous = current;
            }

            for (var i = 0; i < length; i++)
            {
                var (node, randomIndex) = nodeAndRandomIndexByIndexMap[i];

                if (randomIndex == -1)
                {
                    continue;
                }

                var (randomNode, _) = nodeAndRandomIndexByIndexMap[randomIndex];
                node.Random = randomNode;
            }

            dummyHead.Next.Previous = null;

            return Task.FromResult(dummyHead.Next);
        }

        public async Task Serialize(ListNode head, Stream s)
        {
            var length = 0;
            var indexesMap = new Dictionary<ListNode, int>();
            var current = head;

            while (current is not null)
            {
                indexesMap[current] = length++;
                current = current.Next;
            }

            await using var writer = new BinaryWriter(s, Encoding.UTF8);
            writer.Write(length);

            current = head;

            while (current is not null)
            {
                writer.Write(current.Data);
                writer.Write(current.Random is null ? -1 : indexesMap[current.Random]);
                current = current.Next;
            }
        }
    }
}