using UnityEngine;
using XIV.Ecs;
using XIV.UnityEngineIntegration;

namespace TheGame
{
    public struct NodeStateManagementUIComp : IComponent
    {
        public Entity highlightedNodeEntity;
        public RectTransform container;
    }

    public class SerializedNodeStateManagementUIComp : SerializedComponent<NodeStateManagementUIComp>
    {
        public RectTransform uiContainer;

        public override void AddComponentForEntity(Entity entity)
        {
            component.container = uiContainer;
            base.AddComponentForEntity(entity);
        }
    }
}