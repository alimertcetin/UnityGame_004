using System;
using XIV.Core.Collections;

namespace XIV.Ecs
{
    public delegate void CustomReset<T>(ref T component);

    public delegate void CustomAssign<T>(ref T component, T value);
    
    public class ComponentPool<T> : ComponentPoolBase where T : struct
    {
        // public override Type ComponentType => typeof(T);
        public readonly DynamicArray<T> components;

        public CustomReset<T> customReset = null;
        public CustomAssign<T> customAssign = null;

        public ComponentPool()
        {
            components = new DynamicArray<T>(64);
        }
        
        [Obsolete]
        public override object Get(int idx)
        {
            return components[idx];
        }
        
        [Obsolete]
        public override void Set(int idx, object value)
        {
            components[idx] = (T)value;
        }

        // typed accessor (hot-path)
        public ref T GetRef(int idx) => ref components[idx];

        // typed Set (hot-path) - replace object-based Set in generic callers
        public void Set(int idx, in T value)
        {
            components[idx] = value;
        }

        // typed SetNewComponent (hot-path) - set for a freshly allocated slot
        public void SetNewComponent(int idx, in T value)
        {
            if (customAssign != null)
            {
                customAssign(ref components[idx], value);
                return;
            }
            components[idx] = value;
        }

        public override Type ComponentType => typeof(T);
        
        public override void AddComponent()
        {
            components.Add();
        }

        // Copy typed value into dest pool if it's of same T; otherwise throw (shouldn't happen).
        public override void CopyTo(int srcIdx, ComponentPoolBase destPool, int destIdx)
        {
            if (destPool is ComponentPool<T> dest)
            {
                T value = components[srcIdx];
                if (dest.customAssign != null)
                    dest.customAssign(ref dest.components[destIdx], value);
                else
                    dest.components[destIdx] = value;
            }
            else
            {
                throw new InvalidOperationException($"Incompatible pool types: {typeof(T)} -> {destPool.ComponentType}");
            }
        }

        public override void SetCustomReset(Delegate resetFunc)
        {
            customReset = (CustomReset<T>)resetFunc;
        }
        
        public override void SetCustomAssign(Delegate assignFunc)
        {
            customAssign = (CustomAssign<T>)assignFunc;
        }
        
        public void RemoveAll()
        {
            if (customReset != null)
            {
                for (int i = 0; i < components.Count; i++)
                {
                    customReset(ref components[i]);
                }
            }
            else
            {
                for (int i = 0; i < components.Count; i++)
                {
                    components[i] = default;
                }
            }
        
            components.Clear();
        }
        
        public override void SwapRemoveComponentAtIndex(int idx, bool reset = true)
        {
            if (reset) customReset?.Invoke(ref components[idx]);
            components[idx] = components.RemoveLast();
        }
    }
}