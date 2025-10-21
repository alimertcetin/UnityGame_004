using System;
using System.Collections.Generic;
using System.Text;
using XIV.Core.Extensions;

namespace XIV.Ecs
{
    static class TagId<T> where T : struct, ITag
    {
        public static int id;
    }
    public static class TagIdManager
    {
        public static int NumberOfTags => numberOfTags;

        static int numberOfTags = -1;
        static Type[] idToType;
        static TypeIdHashTable typeToId;

        public static void Init(TypeManager typeManager)
        {
            if (numberOfTags != -1)
            {
                // Initialized already
                return;
            }

            var tagTypes = typeManager.GetTagTypes();
            // Debug.Log("Number of Tags:" + tagTypes.Count);

            idToType = new Type[tagTypes.Count];
            typeToId = new TypeIdHashTable(tagTypes.Count * 4);
            numberOfTags = tagTypes.Count;

            for (int tagId = 0; tagId < tagTypes.Count; tagId++)
            {
                var tagType = tagTypes[tagId];
                idToType[tagId] = tagType;
                typeToId.Add(tagType, tagId);

                var tagIdType = typeof(TagId<>).MakeGenericType(tagType);
                tagIdType.XIVSetField("id", null, tagId);
                // tagIdType.DalSet(null, "entityId", tagId);
            }

        }

        public static int GetTagId<T>() where T : struct, ITag
        {
            return TagId<T>.id;
        }

        public static int GetTagId(Type type)
        {
            return typeToId.GetId(type);
        }

        public static string GetTagNames(IEnumerable<int> tagIds, StringBuilder builder)
        {
            builder.Clear();
            foreach (var id in tagIds)
            {
                builder.Append(idToType[id].Name);
                builder.Append(",");
            }

            return builder.ToString();
        }

        public static Type GetTagType(int tagId)
        {
            return idToType[tagId];
        }

        public static string GetTagName(int tagId)
        {
            return idToType[tagId].Name;
        }
    }
}