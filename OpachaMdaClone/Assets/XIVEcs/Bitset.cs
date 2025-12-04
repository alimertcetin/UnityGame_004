using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics; // for BitOperations.PopCount (hardware-accelerated)

namespace XIV.Ecs
{
    public struct Bitset : ICloneable, IEquatable<Bitset>, IEnumerable<int>
    {
        public const int BITS_PER_BUCKET = sizeof(int) * 8; // 32 bits
        public int[] buckets;

        // -----------------------------
        // Internal helpers
        // -----------------------------

        static int GetBucketIndex(int idx) => idx / BITS_PER_BUCKET;
        static int GetBitPosition(int idx) => idx % BITS_PER_BUCKET;

        void EnsureCapacity(int bitIndex)
        {
            int neededBucket = GetBucketIndex(bitIndex);

            if (buckets == null)
            {
                buckets = new int[Math.Max(1, neededBucket + 1)];
                return;
            }

            if (neededBucket >= buckets.Length)
            {
                Array.Resize(ref buckets, neededBucket + 1);
            }
        }

        // -----------------------------
        // Bit operations
        // -----------------------------

        public bool IsBit1(int i)
        {
            int bucketIdx = GetBucketIndex(i);
            if (bucketIdx >= buckets.Length) return false;
            int bitPos = GetBitPosition(i);
            return (buckets[bucketIdx] & (1 << bitPos)) != 0;
        }

        public void SetBit1(int i)
        {
            if (i < 0) return;
            EnsureCapacity(i);
            int bucketIdx = GetBucketIndex(i);
            int bitPos = GetBitPosition(i);
            buckets[bucketIdx] |= 1 << bitPos;
        }

        public void SetBit0(int i)
        {
            if (i < 0) return;
            int bucketIdx = GetBucketIndex(i);
            if (bucketIdx >= buckets.Length) return; // nothing to clear
            int bitPos = GetBitPosition(i);
            buckets[bucketIdx] &= ~(1 << bitPos);
        }

        public void Clear()
        {
            if (buckets == null) return;
            Array.Clear(buckets, 0, buckets.Length);
        }

        // -----------------------------
        // Set/Subset logic
        // -----------------------------

        public bool IsSubsetOf(ref Bitset other)
        {
            // If other has fewer buckets, it cannot contain this set
            if (other.buckets.Length < buckets.Length)
                return false;

            for (int i = 0; i < buckets.Length; i++)
            {
                int a = buckets[i];
                int b = other.buckets[i];

                // a must be fully contained in b
                if ((a & b) != a)
                    return false;
            }

            return true;
        }

        public bool AnyMatchingBits(ref Bitset other)
        {
            int count = Math.Min(buckets.Length, other.buckets.Length);
            for (int i = 0; i < count; i++)
            {
                if ((buckets[i] & other.buckets[i]) != 0)
                    return true;
            }
            return false;
        }

        // -----------------------------
        // Bit counting
        // -----------------------------

        public int GetSetBitCount()
        {
            int count = 0;

            if (buckets == null) return 0;

#if NET5_0_OR_GREATER
            // Hardware accelerated (POPCNT)
            for (int i = 0; i < buckets.Length; i++)
                count += BitOperations.PopCount((uint)buckets[i]);
#else
            // Fallback (slower)
            foreach (int bitIndex in this)
                count++;
#endif
            return count;
        }

        // -----------------------------
        // Clone / Copy
        // -----------------------------

        public static Bitset Copy(ref Bitset bitset)
        {
            var newSet = new Bitset
            {
                buckets = (int[])bitset.buckets.Clone()
            };
            return newSet;
        }

        public object Clone() => Copy(ref this);

        // -----------------------------
        // Comparison
        // -----------------------------

        public bool Equals(Bitset other) => this == other;

        public bool Equals(ref Bitset other)
        {
            if (buckets.Length != other.buckets.Length) return false;

            for (int i = 0; i < buckets.Length; i++)
            {
                if (buckets[i] != other.buckets[i])
                    return false;
            }

            return true;
        }

        public static bool operator ==(Bitset a, Bitset b)
        {
            if (a.buckets.Length != b.buckets.Length) return false;

            for (int i = 0; i < a.buckets.Length; i++)
            {
                if (a.buckets[i] != b.buckets[i])
                    return false;
            }

            return true;
        }

        public static bool operator !=(Bitset a, Bitset b) => !(a == b);

        public override bool Equals(object obj) =>
            obj is Bitset other && this == other;

        // MUCH better hash code than before
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                for (int i = 0; i < buckets.Length; i++)
                    hash = hash * 31 + buckets[i];
                return hash;
            }
        }

        // -----------------------------
        // Enumerator
        // -----------------------------

        public IEnumerator<int> GetEnumerator()
        {
            if (buckets == null) yield break;

            for (int bucketIndex = 0; bucketIndex < buckets.Length; bucketIndex++)
            {
                int bucket = buckets[bucketIndex];
                if (bucket == 0) continue;

                for (int bit = 0; bit < BITS_PER_BUCKET; bit++)
                {
                    int mask = 1 << bit;
                    if ((bucket & mask) != 0)
                    {
                        yield return bucketIndex * BITS_PER_BUCKET + bit;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
