using System;

namespace XIV.Ecs
{
    public interface IComponentPool
    {
        public void SwapRemoveMovedComponent(int idx);
        public void SwapRemoveComponentAtIndex(int idx);
        public void IncreaseCapacity(int amount);
        public void AddComponent();
    
        // TODO : Avoid boxing
        public object Get(int idx);
        public void Set(int idx, object value);
        public void SetNewComponent(int idx, object value);
    
        public void SetCustomReset(object resetFunc);
        public void SetCustomAssign(object assignFunc);
    
        public void RemoveAll();
    
    }
    // public abstract class ComponentPoolBase
    // {
    //     public abstract Type ComponentType { get; }
    //
    //     // Capacity/management
    //     public abstract void IncreaseCapacity(int amount);
    //     public abstract void AddComponent();
    //
    //     // Remove / swap operations
    //     // Component is already moved, just remove it
    //     public abstract void SwapRemoveMovedComponent(int idx);
    //     // Component is not moved, remove it from index
    //     public abstract void SwapRemoveComponentAtIndex(int idx);
    //
    //     // Copy value from this pool[srcIdx] into destPool[destIdx].
    //     // Implemented by generic subclass to do a typed copy without boxing.
    //     public abstract void CopyTo(int srcIdx, ComponentPoolBase destPool, int destIdx);
    //
    //     // Move value from this pool[srcIdx] into destPool[destIdx] then perform swap-remove in source.
    //     // Optional; you can implement with CopyTo + SwapRemoveMovedComponent in ArchetypeMap, but it's often useful.
    //     public abstract void MoveToAndSwapRemove(int srcIdx, ComponentPoolBase destPool, int destIdx);
    //
    //     // Set custom delegates (uses Delegate to avoid boxing)
    //     public abstract void SetCustomReset(Delegate reset);
    //     public abstract void SetCustomAssign(Delegate assign);
    //
    //     public abstract void RemoveAll();
    // }

}