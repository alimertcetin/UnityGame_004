namespace XIV.Ecs
{
    public static class BitSetFactory
    {
        static int GetBitBucketSize(int numberOfItems)
        {
            var len = numberOfItems / Bitset.BITS_PER_BUCKET;
            if (numberOfItems % Bitset.BITS_PER_BUCKET != 0)
            {
                len++;
            }

            return len;
        }


        public static Bitset CreateComponentBitSet()
        {
            return new Bitset
            {
                buckets = new int[GetBitBucketSize(ComponentIdManager.ComponentTypeCount)]
            };
        }

        public static Bitset CreateTagBitSet()
        {
            return new Bitset
            {
                buckets = new int[GetBitBucketSize(TagIdManager.NumberOfTags)]
            };
        }

    }
}