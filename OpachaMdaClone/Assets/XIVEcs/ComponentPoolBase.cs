using System;

namespace XIV.Ecs
{
    public abstract class ComponentPoolBase
    {
        public abstract Type ComponentType { get; }
        
        public abstract void AddComponent();
        
        // Component is not moved, remove it from index
        public abstract void SwapRemoveComponentAtIndex(int idx, bool reset = true);
    
        // Copy value from this pool[srcIdx] into destPool[destIdx].
        // Implemented by generic subclass to do a typed copy without boxing.
        public abstract void CopyTo(int srcIdx, ComponentPoolBase destPool, int destIdx);
        
        [Obsolete]
        public abstract object Get(int idx);
        
        [Obsolete]
        public abstract void Set(int idx, object value);
        
    
        // Set custom delegates (uses Delegate to avoid boxing)
        public abstract void SetCustomReset(Delegate reset);
        public abstract void SetCustomAssign(Delegate assign);
    }

}