using XIV.Ecs;

namespace TheGame
{
    public class NodeSelectionSystem : XIV.Ecs.System
    {
        readonly Filter<PlayerIntentSelectNodeEventComp> selectNodeEventFilter = null;
        readonly Filter<PlayerIntentDeselectNodeEventComp> deselectNodeEventFilter = null;
        readonly Filter<NodeSelectionListenerComp> nodeSelectionListenerFilter = null;

        public override void Update()
        {
            Iterator.Iterate(selectNodeEventFilter, nodeSelectionListenerFilter, 
                (Entity eventEntity, ref PlayerIntentSelectNodeEventComp selectNodeEventComp,
                Entity selectionListenerEntity, ref NodeSelectionListenerComp nodeSelectionListenerComp) =>
            {
                var targetNodeEntity = selectNodeEventComp.targetNodeEntity;
                nodeSelectionListenerComp.previousSelectedNode = nodeSelectionListenerComp.selectedNode;
                nodeSelectionListenerComp.selectedNode = targetNodeEntity;
                nodeSelectionListenerComp.version++;
            });
            
            Iterator.Iterate(deselectNodeEventFilter, nodeSelectionListenerFilter,
                (Entity eventEntity, ref PlayerIntentDeselectNodeEventComp deselectNodeEventComp,
                    Entity selectionListenerEntity, ref NodeSelectionListenerComp nodeSelectionListenerComp) =>
                {
                    nodeSelectionListenerComp.previousSelectedNode = nodeSelectionListenerComp.selectedNode;
                    nodeSelectionListenerComp.selectedNode = Entity.Invalid;
                    nodeSelectionListenerComp.version++;
                });
            
            selectNodeEventFilter.DestroyAll();
            deselectNodeEventFilter.DestroyAll();
        }
    }

}