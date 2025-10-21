using System;
using System.Collections;
using System.Collections.Generic;

namespace XIV.Ecs
{
    public struct Bitset : ICloneable, IEquatable<Bitset>, IEnumerable<int>
    {
        public const int MAX_SET_SIZE = sizeof(int) * 8; // sizeof returns the number bytes
        public int[] buckets;

        static int GetBucketIndex(int idx) => idx / MAX_SET_SIZE;
        static int GetBitPosition(int idx) => idx % MAX_SET_SIZE;

        public bool IsBit1(int i)
        {
            var bucketIdx = GetBucketIndex(i);
            var bitPosition = GetBitPosition(i);
            return (buckets[bucketIdx] & (1 << bitPosition)) != 0;
        }

        public void SetBit1(int i)
        {
            var bucketIdx = GetBucketIndex(i);
            var bitPosition = GetBitPosition(i);
            buckets[bucketIdx] |= 1 << bitPosition;
        }

        public void SetBit0(int i)
        {
            var bucketIdx = GetBucketIndex(i);
            var bitPosition = GetBitPosition(i);
            buckets[bucketIdx] &= ~(1 << bitPosition);
        }

        public void Clear()
        {
            int length = buckets.Length;
            for (int i = 0; i < length; i++)
            {
                buckets[i] = 0;
            }
        }
        
        public bool IsSubsetOf(ref Bitset other) 
        {
            var otherBuckets = other.buckets;
            var bucketsLength = buckets.Length;
            for (int i = 0; i < bucketsLength; i++) 
            {
                var set = buckets[i];
                if ((set & otherBuckets[i]) != set)
                {
                    return false;
                }
            }
            return true;
        }
        
        /// Determines whether any of the bits in this instance are also set in the given bitset.
        public bool AnyMatchingBits(ref Bitset other)
        {
            var otherBuckets = other.buckets;
            var bucketsLength = buckets.Length;
            for (int i = 0; i < bucketsLength; i++) 
            {
                var bit = buckets[i];
                if ((bit & otherBuckets[i]) != 0)
                {
                    return true;
                }
            }

            return false;
        }
        
        public int GetSetBitCount()
        {
            int count = 0;
            foreach (var set in this)
            {
                count++;
            }
            return count;
        }

        public static Bitset Copy(ref Bitset bitset)
        {
            int len = bitset.buckets.Length;
            var newSet = new Bitset
            {
                buckets = new int[len],
            };
            for (int i = 0; i < len; i++)
            {
                newSet.buckets[i] = bitset.buckets[i];
            }

            return newSet;
        }

        public object Clone()
        {
            return Copy(ref this);
        }

        public static bool operator ==(Bitset a, Bitset b)
        {
            var aBuckets = a.buckets;
            var bBuckets = b.buckets;
            int aBucketLen = aBuckets.Length;
            if (aBucketLen != bBuckets.Length) return false;
            for (int i = 0; i < aBucketLen; i++)
            {
                if (bBuckets[i] != aBuckets[i]) return false;
            }
            return true;
        }

        public static bool operator !=(Bitset a, Bitset b)
        {
            return !(a == b);
        }

        public bool Equals(Bitset other)
        {
            return this == other;
        }

        public bool Equals(ref Bitset other)
        {
            var aBuckets = this.buckets;
            var bBuckets = other.buckets;
            int aBucketLen = aBuckets.Length;
            if (aBucketLen != bBuckets.Length) return false;
            for (int i = 0; i < aBucketLen; i++)
            {
                if (bBuckets[i] != aBuckets[i]) return false;
            }
            return true;
        }

        public IEnumerator<int> GetEnumerator()
        {
            for (int bucketIdx = 0; bucketIdx < buckets.Length; bucketIdx++)
            {
                int set = buckets[bucketIdx];
                if (set == 0) continue;
                
                for (int j = 0; j < MAX_SET_SIZE; j++)
                {
                    if ((set & (1 << j)) != 0)
                    {
                        yield return bucketIdx * MAX_SET_SIZE + j;
                    }
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Bitset other && this == other;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            var len = buckets.Length;
            for (int i = 0; i < len; i++)
            {
                hash |= buckets[i];
            }
            return hash;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}