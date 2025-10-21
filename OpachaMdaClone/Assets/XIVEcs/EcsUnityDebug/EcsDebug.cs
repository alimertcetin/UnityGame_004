using UnityEngine;

namespace XIV.Ecs
{
#if UNITY_EDITOR
   
   public class EcsDebug : MonoBehaviour
   {
      public static GameObject CreateDebug(World world,SystemManager manager, string name)
      {
         GameObject debug = new GameObject(name);
         debug.AddComponent<EcsDebug>().Setup(world,manager);
         return debug;
      }
      
      Transform entityContainer;
      Transform filterContainer;
      Transform archetypeContainer;
      World world;
      SystemManager systemManager;
      
      void Setup(World w,SystemManager manager)
      {
         DontDestroyOnLoad(gameObject);
         world = w;
         systemManager = manager;

         var t = transform;
         entityContainer = new GameObject("Entities")
         {
            transform = { parent = t }
         }.transform;

         filterContainer = new GameObject("Filters")
         {
            transform = {parent = t}
         }.transform;

         archetypeContainer = new GameObject("Archetypes")
         {
            transform = {parent = t}
         }.transform;

      }

      public void RefreshWorld()
      {
         UpdateEntities();
         UpdateFilters();
         UpdateArchetypes();
      }

      void UpdateEntities()
      {
         int nEntities = 0;
         foreach (var archetype in world.archetypeMap.archetypes)
         {
            nEntities += archetype.entities.Count;
         }
         
         entityContainer.name = "Entities: " + nEntities;
         UpdateChildCountEntity(entityContainer,nEntities);

         int childIdx = 0;
         foreach (var archetype in world.archetypeMap.archetypes)
         {
            for (int i = 0; i < archetype.entities.Count; i++)
            {
               var entity = archetype.entities[i];
               GameObject go = entityContainer.GetChild(childIdx++).gameObject;
               go.name = new Entity(world, entity.entityId.id, entity.entityId.generation).ToString();
               go.GetComponent<DebugEntityMono>().entity = entity;
            }
         }
         
      }

      void UpdateFilters()
      {
         int nFilter = world.filters.Count;
         var filters = world.filters;
         filterContainer.name = "Filters: " + nFilter;
         UpdateChildCount(filterContainer,nFilter);

         int filterIdx = -1;
         foreach (var filter in filters)
         {
            filterIdx++;
            Transform filterTransform = filterContainer.GetChild(filterIdx);
            filterTransform.name = filters[filterIdx].ToString();
            UpdateChildCountEntity(filterTransform,filters[filterIdx].NumberOfEntities);

            int entityIdx = -1;
            filter.ForEach((Entity entity) =>
            {
               entityIdx++;
               Transform entityTransform = filterTransform.GetChild(entityIdx);
               if (!entity.IsAlive())
               {
                  return;
               }
               entityTransform.name = entity.ToString();
               if (entity.HasComponent<DebugNameComp>())
               {
                  entityTransform.name = entity.GetComponent<DebugNameComp>().name + entityTransform.name;
               }
               entityTransform.GetComponent<DebugEntityMono>().entity = entity;
            });

         }
      }
      
      void UpdateArchetypes()
      {
         int nArchetypes = world.archetypeMap.archetypes.Count;
         var archetypes = world.archetypeMap.archetypes;
         archetypeContainer.name = "Archetypes: " + nArchetypes;
         UpdateChildCount(archetypeContainer,nArchetypes);

         int archetypeIdx = -1;
         foreach (var archetype in archetypes)
         {
            archetypeIdx++;
            Transform archetypeTransform = archetypeContainer.GetChild(archetypeIdx);
            archetypeTransform.name = archetype.ToString();
            UpdateChildCountEntity(archetypeTransform,archetype.entities.Count);

            for (int i = 0;  i< archetype.entities.Count; i++)
            {
               Transform entityTransform = archetypeTransform.GetChild(i);
               var entity = archetype.entities[i];
               entityTransform.name = entity.ToString();
               if (entity.HasComponent<DebugNameComp>())
               {
                  entityTransform.name = entity.GetComponent<DebugNameComp>().name + entityTransform.name;
               }
               entityTransform.GetComponent<DebugEntityMono>().entity = entity; 
            };
         }
      }
      
      static void UpdateChildCount(Transform containerTransform, int nChild)
      {
         int childCount = containerTransform.childCount;
         for (int i = 0; i < childCount - nChild; i++)
         {
            var go = containerTransform.GetChild(containerTransform.childCount - 1).gameObject;
            DestroyImmediate(go);
         }
         
         childCount = containerTransform.childCount;
           
         for (int i = 0; i < nChild - childCount; i++)
         {
            new GameObject("---")
            {
               transform =
               {
                  parent = containerTransform.transform
               }
            };
         }
         
      }
      
      static void UpdateChildCountEntity(Transform containerTransform, int nChild)
      {
         int childCount = containerTransform.childCount;
         for (int i = 0; i < childCount - nChild; i++)
         {
            var go = containerTransform.GetChild(containerTransform.childCount - 1).gameObject;
            DestroyImmediate(go);
         }
         
         childCount = containerTransform.childCount;
           
         for (int i = 0; i < nChild - childCount; i++)
         {
            GameObject entity = new GameObject("---");
            entity.AddComponent<DebugEntityMono>();
            entity.transform.parent = containerTransform.transform;
         }
      }

      void Update()
      {
         world.CheckLeakedEntities();
      }
   }
#endif

}
