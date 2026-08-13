using XIV.Core.Collections;
using XIV.Core.Utils;
using XIV.Ecs;

namespace TheGame
{
    public struct PathFinderComp : IComponent
    {
        public DynamicArray<Entity> path;
    }
    
    public class NodePathFindSystem : XIV.Ecs.System
    {
        readonly Filter<OccupiedNodeComp, PathFinderComp> pathFinderFilter = null;
        // readonly Filter<OccupiedNodeComp, PathFollowerComp> pathFollowerFilter = null;
        readonly ConnectionDB connectionDB = null;
        Timer pathFindTimer = new Timer(2f);

        public override void Start()
        {
            NodePathFinder.Init();
        }

        public override void Update()
        {
            if (pathFindTimer.Update(XTime.deltaTime) == false) return;
            pathFindTimer.Restart();
            
            pathFinderFilter.ForEach((Entity entity, ref OccupiedNodeComp occupiedNodeComp, ref PathFinderComp pathFinderComp) =>
            {
                NodePathFinder.GetPathToFirstTarget(entity, connectionDB, GameConstants.MAX_RESOURCE_QUANTITY, ref pathFinderComp.path);
            });
            
        }
    }
}