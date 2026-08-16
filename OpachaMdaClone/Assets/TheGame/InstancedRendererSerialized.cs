using UnityEngine;
using XIV.Ecs;
using XIV.UnityEngineIntegration;

namespace TheGame
{
    public struct InstancedRendererComp : IComponent
    {
        public Renderer renderer;
        public MaterialPropertyBlock materialPropertyBlock;
    }

    public class InstancedRendererSerialized : SerializedComponent<InstancedRendererComp>
    {
        public Renderer renderer;
        
        public override void AddComponentForEntity(Entity entity)
        {
            component.renderer = renderer;
            component.materialPropertyBlock = new MaterialPropertyBlock();
            component.renderer.GetPropertyBlock(component.materialPropertyBlock);
            entity.AddComponent(component);
        }

        void OnValidate()
        {
            renderer ??= GetComponent<Renderer>();
            renderer ??= GetComponentInChildren<Renderer>();
        }
    }
}