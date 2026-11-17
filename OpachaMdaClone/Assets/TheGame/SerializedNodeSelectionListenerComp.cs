using System;
using XIV.Ecs;
using XIV.UnityEngineIntegration;

namespace TheGame
{
    public struct NodeSelectionListenerComp : IComponent
    {
        public Entity previousSelectedNode;
        public Entity selectedNode;
        public uint version;
    }

    public class SerializedNodeSelectionListenerComp : SerializedComponent<NodeSelectionListenerComp>
    {
        
    }
}