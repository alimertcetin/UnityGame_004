using System;

namespace XIV.Ecs
{
    public class TypeIdHashTable
    {
        public class Node
        {
            public Type type;
            public int id;
            public Node next;
        }

        public Node[] buckets;

        // TODO if we give the type array on initialization we can further optimize bucket size
        // therefore the collision number

        // TODO send all types in a list, detect collisions beforehand and create lists inside nodes instead of linked list approach

        public TypeIdHashTable(int nBuckets)
        {
            buckets = new Node[nBuckets];
        }

        public void Add(Type type, int id)
        {
            int hash = type.GetHashCode();
            if (hash < 0) hash = -hash;

            int bucketIdx = hash % buckets.Length;
            var node = buckets[bucketIdx];

            if (node == null)
            {
                buckets[bucketIdx] = new Node()
                {
                    type = type,
                    id = id,
                    next = null
                };
                return;
            }

            while (node.next != null) node = node.next;

            node.next = new Node
            {
                type = type,
                id = id,
                next = null
            };
        }

        public int GetId(Type type)
        {
            int hash = type.GetHashCode();
            if (hash < 0)
            {
                hash = -hash;
            }

            var node = buckets[hash % buckets.Length];

            while (node.type != type)
            {
                node = node.next;
            }

            return node.id;
        }

    }
}