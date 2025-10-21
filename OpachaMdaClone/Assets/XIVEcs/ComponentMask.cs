using System;

namespace XIV.Ecs
{
    public class ComponentMask
    {
        public Bitset includeComponentSet = BitSetFactory.CreateComponentBitSet();
        public Bitset excludeComponentSet = BitSetFactory.CreateComponentBitSet();

        public Bitset includeTagSet = BitSetFactory.CreateTagBitSet();
        public Bitset excludeTagSet = BitSetFactory.CreateTagBitSet();

        public void IncludeComponent(int componentPoolIdx)
        {
            includeComponentSet.SetBit1(componentPoolIdx);
        }

        public void ExcludeComponent(int componentPoolIdx)
        {
            excludeComponentSet.SetBit1(componentPoolIdx);
        }

        public void IncludeTag(int tagIdx)
        {
            includeTagSet.SetBit1(tagIdx);
        }

        public void ExcludeTag(int tagIdx)
        {
            excludeTagSet.SetBit1(tagIdx);
        }

        public bool IsCompatible(Bitset componentBitSet, Bitset tagBitSet)
        {
            return
                includeComponentSet.IsSubsetOf(ref componentBitSet)
                && !excludeComponentSet.AnyMatchingBits(ref componentBitSet)
                && includeTagSet.IsSubsetOf(ref tagBitSet)
                && !excludeTagSet.AnyMatchingBits(ref tagBitSet);
        }

        // TODO remove GetHashCode and Equals after writing a custom dictionary for filter injection
        public override int GetHashCode()
        {
            return HashCode.Combine(includeComponentSet.GetHashCode(), excludeComponentSet.GetHashCode(),
                includeTagSet.GetHashCode(), excludeTagSet.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is ComponentMask otherMask))
            {
                return false;
            }

            return includeComponentSet.Equals(ref otherMask.includeComponentSet)
                   && excludeComponentSet.Equals(ref otherMask.excludeComponentSet)
                   && includeTagSet.Equals(ref otherMask.includeTagSet)
                   && excludeTagSet.Equals(ref otherMask.excludeTagSet);
        }
    }
}