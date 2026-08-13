using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using XIV.Ecs;

namespace TheGame
{
    public class GameEndDetectionSystem : XIV.Ecs.System
    {
        readonly Filter<OccupiedNodeComp> occupiedNodeFilter = null;
        readonly Filter<TransferableResourceComp> transferableResourceFilter = null;

        public override void Update()
        {
            HashSet<Entity> unitEntities = HashSetPool<Entity>.New();
            
            occupiedNodeFilter.ForEach((Entity occupiedNodeEntity, ref OccupiedNodeComp occupiedNodeComp) =>
            {
                unitEntities.Add(occupiedNodeComp.unitEntity);
            });

            transferableResourceFilter.ForEach((Entity transferableResourceEntity, ref TransferableResourceComp transferableResourceComp) =>
            {
                unitEntities.Add(transferableResourceComp.unitEntity);
            });
            
            var isGameEnded = unitEntities.Count == 1;

            if (isGameEnded
                #if UNITY_EDITOR
                || Input.GetKeyDown(KeyCode.Space)
                #endif
                )
            {
                manager.ChangeState(EasyLevelController.States.LevelCompleted);
            }
            
            HashSetPool<Entity>.Free(unitEntities);
        }
    }
}