using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;
using XIV.Core.Extensions;

namespace XIV.Ecs
{
    [AttributeUsage((AttributeTargets.Method))]
    public class OnResetAttribute : Attribute {}
    
    public abstract class SerializedComponent : MonoBehaviour
    {
        public bool add = true;

        public abstract void AddComponentForEntity(Entity entity);
        
        public abstract int GetComponentId();

        void Reset()
        {
            // if gameObject has GameObjectEntity AddComponent will return null, will not change existing one
            gameObject.AddComponent<GameObjectEntity>();
            GetType().XIVInvokeMethodsHasAttribute<OnResetAttribute>(this);
        }
    }

    public abstract class SerializedTag : MonoBehaviour
    {
        public abstract int GetTagId();

        void Reset()
        {
            // if gameObject has GameObjectEntity AddComponent will return null, will not change existing one
            gameObject.AddComponent<GameObjectEntity>();
        }
    }
    

    public abstract class SerializedComponent<TComponentType> : SerializedComponent
        where TComponentType : struct, IComponent
    {
        public TComponentType component;

        public override void AddComponentForEntity(Entity entity)
        {
            entity.AddComponent<TComponentType>(component);
        }

        public sealed override int GetComponentId()
        {
            return ComponentIdManager.GetComponentId<TComponentType>();
        }

        // Has to be public because of reflection
        [OnReset]
        public void CallCustomReset()
        {
            BindingFlags flags = BindingFlags.Public 
                    | BindingFlags.NonPublic 
                    | BindingFlags.Instance 
                    | BindingFlags.Static;
            
            var componentType = typeof(TComponentType);
            var methods = componentType.GetMethods(flags);
            foreach (var method in methods)
            {
                if (method.GetCustomAttribute<OnResetAttribute>() == null) continue;
                try
                {
                    component = (TComponentType)method.Invoke(component, null);
                }
                catch
                {
                    Debug.LogError($"{componentType.Name}-{method.Name}-Function cannot take any parameters and has to return TComponentType struct with default values");
                }
            }
        }

        [Preserve]
        void AOTFix()
        {
            Debug.Log(new ComponentPool<TComponentType>());
        }
    }

    [RequireComponent(typeof(GameObjectEntity))]
    public abstract class SerializedAction : MonoBehaviour
    {
        public abstract void Action(World world,Entity entity);
    }
    
    public class SerializedTag<T> : SerializedTag
        where T : struct, ITag
    {
        public sealed override int GetTagId()
        {
            return TagIdManager.GetTagId<T>();
        }
    }
   
} 

