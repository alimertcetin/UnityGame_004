using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace XIV.Ecs
{
    public static class EntityExtensions
    {
        public static Transform GetTransform(this Entity entity)
        {
            return entity.GetComponent<TransformComp>().transform;
        }
        
        public static RectTransform GetRectTransform(this Entity entity)
        {
            return (RectTransform)entity.GetTransform();
        }
        public static GameObject GetGameObject(this Entity entity)
        {
            var transform = entity.GetTransform();
            return transform == null ? null : transform.gameObject;
        }

        public static IEnumerable<Transform> GetChildren(this Entity entity)
        {
            var transform = entity.GetTransform();
            foreach (Transform t in transform)
            {
                yield return t;
            }
        }

        public static Transform GetParentTransform(this Entity entity)
        {
            return entity.GetTransform().parent;
        }

        public static Transform GetFirstChildTransform(this Entity entity)
        {
            return entity.GetTransform().GetChild(0);
        }
        
        public static GameObject GetParentGameObject(this Entity entity)
        {
            return entity.GetTransform().parent.gameObject;
        }

        public static GameObject GetFirstChildGameObject(this Entity entity)
        {
            return entity.GetTransform().GetChild(0).gameObject;
        }

        public static void TryAddComponent<T>(this Entity entity, T component) where T : struct, IComponent
        {
            if (entity.HasComponent<T>()) return;
            entity.AddComponent(component);
        }
        
        public static bool TryGetComponent<T>(this Entity entity, ref T component) where T : struct, IComponent
        {
            if (!entity.HasComponent<T>()) return false;
            component = entity.GetComponent<T>();
            return true;
        }
        
        public static void Highlight(this Entity entity)
        {
#if UNITY_EDITOR
            UnityEditor.EditorGUIUtility.PingObject(entity.GetGameObject());
#endif
        }

        public static void SelectFromHierarchy(this Entity entity)
        {
#if UNITY_EDITOR
            UnityEditor.Selection.activeObject = entity.GetGameObject();
#endif
        }

        public static IComponent[] GetComponents(this Entity entity)
        {
            var archetype = entity.GetArchetype();
            IComponent[] components = new IComponent[archetype.componentIds.Length];
            ref var entityData = ref entity.world.entityDataList[entity.entityId.id];
            int archetypeIdx = entityData.archetypeIndex;

            for (int i = 0; i < components.Length; i++)
            {
                var componentId = archetype.componentIds[i];
                components[i] = (IComponent)archetype.GetComponentPool(componentId).Get(archetypeIdx);
            }

            return components;
        }

        public static Type[] GetTags(this Entity entity)
        {
            var archetype = entity.GetArchetype();
            var tags = new Type[archetype.tagBitSet.GetSetBitCount()];

            int i = 0;
            foreach (var tagId in archetype.tagBitSet)
            {
                tags[i++] = TagIdManager.GetTagType(tagId);
            }
            return tags;
        }
        
        public static string GetComponentNames(this Entity entity)
        {
            StringBuilder builder = new StringBuilder();
            var components = entity.GetComponents();

            for (int i = 0; i < components.Length - 1; i++)
            {
                builder.Append(components[i].GetType().Name);
                builder.Append(",");
            }

            if (components.Length != 0)
            {
                builder.Append(components[^1].GetType().Name);
            }
            else
            {
                return "HasNoComponent";
            }

            return builder.ToString();
        }
        
        public static string GetTagNames(this Entity entity)
        {
            StringBuilder builder = new StringBuilder();
            var tags = entity.GetTags();

            for (int i = 0; i < tags.Length - 1; i++)
            {
                builder.Append(tags[i].Name);
                builder.Append(",");
            }

            if (tags.Length != 0)
            {
                builder.Append(tags[^1].Name);
            }
            else
            {
                return "HasNoTag";
            }

            return builder.ToString();
        }

        public static string GetComponentAndTagNames(this Entity entity)
        {
            return $"{GetComponentNames(entity)}-{GetTagNames(entity)}";
        }
        
    }
}