using XIV.Core.Collections;

namespace XIV.Ecs
{
    public struct EntityData
    {
        // generationId is increased by one in each destroy
        // and when an entity is destroyed generationId becomes negative until EntityData is reused
        public int generationId;
        public Archetype archetype;
        public int archetypeIdx;

        public Bitset componentBitSet;
        public Bitset tagBitSet;
        public DynamicArray<DisabledComponent> disabledComponents;
    }
}