using System;
using System.Collections.Generic;
using System.Text;
using XIV.Core.Extensions;

namespace XIV.Ecs
{
    // TODO : Make it non-static
    public static class ComponentIdManager
    {
        static class ComponentId<T> where T : struct, IComponent
        {
            public static int id;
        }
        
        public static int ComponentTypeCount { get; private set; }

        static Type[] idToType;
        static Type[] idToCompPoolType;
        static TypeIdHashTable typeToId;

        public static void Init(TypeManager typeManager)
        {
            // Debug.Log("Number of Components:" + componentTypes.Count);

            var componentTypes = typeManager.GetComponents();
            var componentTypesCount = componentTypes.Count;
            idToType = new Type[componentTypesCount];
            idToCompPoolType = new Type[componentTypesCount];
            typeToId = new TypeIdHashTable(componentTypesCount * 4);
            ComponentTypeCount = componentTypesCount;

            for (int componentId = 0; componentId < componentTypesCount; componentId++)
            {
                var componentType = componentTypes[componentId];

                idToType[componentId] = componentType;
#if UNITY_EDITOR
                if (!componentType.IsValueType)
                {
                    // Debug.LogError($"{componentType.FullName} inherits IComponent but is not a struct");
                }
#endif
                idToCompPoolType[componentId] = typeof(ComponentPool<>).MakeGenericType(componentType);
                typeToId.Add(componentType, componentId);

                var componentIdType = typeof(ComponentId<>).MakeGenericType(componentType);
                componentIdType.XIVSetField("id", componentIdType, componentId);
            }
        }

        [Obsolete]
        public static Type GetComponentPoolType(int componentId) => idToCompPoolType[componentId];
        public static Type GetComponentPoolType<T>() where T : struct, IComponent => idToCompPoolType[ComponentId<T>.id];
        public static int GetComponentId(Type type) => typeToId.GetId(type);
        public static int GetComponentId<T>() where T : struct, IComponent => ComponentId<T>.id;

        public static string GetComponentNames(IEnumerable<int> componentIds, StringBuilder builder)
        {
            builder.Clear();
            foreach (var id in componentIds)
            {
                builder.Append(idToType[id].Name);
                builder.Append(',');
            }

            return builder.ToString();
        }
    }
}