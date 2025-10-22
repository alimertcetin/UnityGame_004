using System;

namespace XIV.Ecs
{
    public struct Entity : IEquatable<Entity>
    {
        public static Entity Invalid = new Entity(null, -1, -1);
        public EntityId entityId;
        public World world;

        public Entity(World world, int id, int generationId)
        {
            this.entityId = new EntityId(id, generationId);
            this.world = world;
        }


        /// Returns false if entity is null or destroyed
        public bool IsAlive()
        {
            return world != null && world.IsEntityAlive(entityId);
        }

        public void AddComponent<T>(T componentValue) where T : struct, IComponent
        {
            world.AddComponent(entityId, componentValue);
        }

        public void RemoveComponent<T>() where T : struct, IComponent
        {
            world.RemoveComponent<T>(entityId);
        }

        public void AddComponents(params IComponent[] componentValues)
        {
            world.AddComponents(entityId, componentValues);
        }

        public void RemoveComponents(params Type[] componentTypes)
        {
            world.RemoveComponents(entityId, componentTypes);
        }

        public ref T GetComponent<T>() where T : struct, IComponent
        {
            return ref world.GetComponent<T>(entityId);
        }

        public bool HasComponent<T>() where T : struct, IComponent
        {
            return world.HasComponent<T>(entityId);
        }

        // TAG
        public void AddTag<T>() where T : struct, ITag
        {
            world.AddTag<T>(entityId);
        }

        public void RemoveTag<T>() where T : struct, ITag
        {
            world.RemoveTag<T>(entityId);
        }

        public bool HasTag<T>() where T : struct, ITag
        {
            return world.HasTag<T>(entityId);
        }

        public void AddTags(params Type[] tagTypes)
        {
            world.AddTags(entityId, tagTypes);
        }

        public void RemoveTags(params Type[] tagTypes)
        {
            world.RemoveTags(entityId, tagTypes);
        }


        public void DisableComponent<T>() where T : struct, IComponent
        {
            world.DisableComponent<T>(entityId);
        }

        public void EnableComponent<T>() where T : struct, IComponent
        {
            world.EnableComponent<T>(entityId);
        }

        public bool HasDisabledComponent<T>() where T : struct, IComponent
        {
            return world.HasDisabledComponent<T>(entityId);
        }

        public void Destroy()
        {
            world.DestroyEntity(entityId);
        }

        public void AddTagAndComponents(object[] componentValues, int[] componentIds, int[] tagIds)
        {
            world.AddComponentsAndTags(entityId, componentValues, componentIds, tagIds);
        }

        public Archetype GetArchetype() => world.GetArchetype(entityId);

        public void AddTagAndComponents(object[] componentValues, Type[] tagTypes)
        {
            // TODO array pool
            int[] componentIds = new int[componentValues.Length];
            int[] tagIds = new int[tagTypes.Length];

            for (int i = 0; i < componentValues.Length; i++)
            {
                componentIds[i] = ComponentIdManager.GetComponentId(componentValues[i].GetType());
            }

            for (int i = 0; i < tagTypes.Length; i++)
            {
                tagIds[i] = TagIdManager.GetTagId(tagTypes[i]);
            }

            world.AddComponentsAndTags(entityId, componentValues, componentIds, tagIds);
        }

        public override string ToString()
        {
            if (world == null)
            {
                return this == Entity.Invalid ? "Invalid Entity" : "Undefined Entity";
            }

            if (IsAlive() && HasComponent<DebugNameComp>())
            {
                return $"{GetComponent<DebugNameComp>().name} {entityId.id},{entityId.generation}";
            }

            return $"Id:{entityId.id}, Generation:{entityId.generation}";
        }

        public void SetName(string name)
        {
            AddComponent(new DebugNameComp() { name = name });
        }

        public string GetName()
        {
            return HasComponent<DebugNameComp>() ? GetComponent<DebugNameComp>().name : "NoName_" + ToString();
        }

        public bool Equals(Entity other)
        {
            return entityId.id == other.entityId.id && entityId.generation == other.entityId.generation;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Entity entity && Equals(entity);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (entityId.id * 397) ^ entityId.generation;
            }
        }

        public static bool operator ==(Entity e1, Entity e2)
        {
            return e1.entityId.id == e2.entityId.id && e1.entityId.generation == e2.entityId.generation;
        }

        public static bool operator !=(Entity e1, Entity e2)
        {
            return !(e1 == e2);
        }
    }
}