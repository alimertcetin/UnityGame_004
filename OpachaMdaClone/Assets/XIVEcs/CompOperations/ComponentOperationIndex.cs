using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using XIV.Core.Collections;
using XIV.Core.Extensions;

namespace XIV.Ecs
{
    public static class ComponentOperationIndex
    {
        // TODO : ComponentOperationIndex -> Just call the necessary operations
        // We are currently executing all actions once execute called. Would be much efficient to call the functions that needs to be executed?
        // What if we store the actions once the call like AddComponent<T> has been made?
        static List<Action<World>> addComponentActions;
        static List<Action<World>> removeComponentActions;
        
        static List<Action<World>> enableComponentActions;
        static List<Action<World>> disableComponentActions;
        
        static List<Action<World>> addTagActions;
        static List<Action<World>> removeTagActions;
        
        static Action<EntityId>[] removeComponentById;
        
        static Action<EntityId>[] addTagById;
        static Action<EntityId>[] removeTagById;
        
        public static void Init(TypeManager typeManager)
        {
            var componentTypes = typeManager.GetComponents();
            var tagTypes = typeManager.GetTagTypes();
            int componentTypeCount = componentTypes.Count;
            int tagTypeCount = tagTypes.Count;

            var componentActionCapacity = componentTypeCount * 2;
            addComponentActions = new List<Action<World>>(componentActionCapacity);
            removeComponentActions = new List<Action<World>>(componentActionCapacity);
            enableComponentActions = new List<Action<World>>(componentActionCapacity);
            disableComponentActions = new List<Action<World>>(componentActionCapacity);
            
            var tagActionCapacity = tagTypeCount * 2;
            addTagActions = new List<Action<World>>(tagActionCapacity);
            removeTagActions = new List<Action<World>>(tagActionCapacity);
            
            removeComponentById = new Action<EntityId>[componentTypeCount];
            
            addTagById = new Action<EntityId>[tagTypeCount];
            removeTagById = new Action<EntityId>[tagTypeCount];

            for (int componentId = 0; componentId < componentTypeCount; componentId++)
            {
                var componentType = componentTypes[componentId];

                var addComponentType = typeof(AddComponentOperations<>).MakeGenericType(componentType);
                var removeComponentType = typeof(RemoveComponentOperations<>).MakeGenericType(componentType);
                var activateComponentType = typeof(ActivateComponentOperations<>).MakeGenericType(componentType);
                addComponentType.XIVGetMethodByName("Init").Invoke(addComponentType, Array.Empty<object>());
                removeComponentType.XIVGetMethodByName("Init").Invoke(removeComponentType, Array.Empty<object>());
                activateComponentType.XIVGetMethodByName("Init").Invoke(activateComponentType, Array.Empty<object>());

                // Pre-build remove delegate once, no reflection at runtime:
                var removeComponentMethod = removeComponentType.XIVGetMethodByName("RemoveComponent");
                removeComponentById[componentId] = (Action<EntityId>)Delegate.CreateDelegate(typeof(Action<EntityId>), removeComponentMethod);
            }

            for (int tagId = 0; tagId < tagTypeCount; tagId++)
            {
                var tagType = tagTypes[tagId];
                var addTagType = typeof(AddTagOperations<>).MakeGenericType(tagType);
                var removeTagType = typeof(RemoveTagOperations<>).MakeGenericType(tagType);
                addTagType.XIVGetMethodByName("Init").Invoke(addTagType, Array.Empty<object>());
                removeTagType.XIVGetMethodByName("Init").Invoke(addTagType, Array.Empty<object>());

                // Pre-build remove delegate once, no reflection at runtime:
                var addTagMethod = addTagType.XIVGetMethodByName("AddTag");
                addTagById[tagId] = (Action<EntityId>)Delegate.CreateDelegate(typeof(Action<EntityId>), addTagMethod);
                var removeTagMethod = removeTagType.XIVGetMethodByName("RemoveTag");
                removeTagById[tagId] = (Action<EntityId>)Delegate.CreateDelegate(typeof(Action<EntityId>), removeTagMethod);
            }

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddComponentAction(Action<World> action, bool isAddAction)
        {
            if (isAddAction) addComponentActions.Add(action);
            else removeComponentActions.Add(action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddActivateComponentAction(Action<World> action, bool isAddAction)
        {
            if (isAddAction) enableComponentActions.Add(action);
            else disableComponentActions.Add(action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddTagAction(Action<World> action, bool isAddAction)
        {
            if (isAddAction) addTagActions.Add(action);
            else removeTagActions.Add(action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddComponent<T>(EntityId entityId, T componentValue) where T : struct, IComponent
        {
            AddComponentOperations<T>.AddComponent(entityId, componentValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveComponent<T>(EntityId entityId) where T : struct, IComponent
        {
            RemoveComponentOperations<T>.RemoveComponent(entityId);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveComponent(EntityId entityId, int componentId)
        {
            removeComponentById[componentId](entityId);
        }

        public static void EnableComponent<T>(EntityId entityId) where T : struct, IComponent
        {
            ActivateComponentOperations<T>.EnableComponent(entityId);
        }

        public static void DisableComponent<T>(World world, EntityId entityId) where T : struct, IComponent
        {
            ActivateComponentOperations<T>.DisableComponent(world, entityId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddTag<T>(EntityId entityId) where T : struct, ITag
        {
            AddTagOperations<T>.AddTag(entityId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddTag(EntityId entityId, int tagId)
        {
            addTagById[tagId](entityId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveTag<T>(EntityId entityId) where T : struct, ITag
        {
            RemoveTagOperations<T>.RemoveTag(entityId);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveTag(EntityId entityId, int tagId)
        {
            removeTagById[tagId](entityId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExecuteAddComponentActions(World world)
        {
            foreach (var action in addComponentActions)
            {
                action(world);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExecuteRemoveComponentActions(World world)
        {
            foreach (var action in removeComponentActions)
            {
                action(world);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExecuteEnableComponentActions(World world)
        {
            foreach (var action in enableComponentActions)
            {
                action(world);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExecuteDisableComponentActions(World world)
        {
            foreach (var action in disableComponentActions)
            {
                action(world);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExecuteAddTagActions(World world)
        {
            foreach (var action in addTagActions)
            {
                action(world);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExecuteRemoveTagActions(World world)
        {
            foreach (var action in removeTagActions)
            {
                action(world);
            }
        }
    }
}