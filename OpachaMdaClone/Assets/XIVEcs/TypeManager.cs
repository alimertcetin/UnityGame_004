using System;
using System.Collections.Generic;
using XIV.Core.Extensions;

namespace XIV.Ecs
{
    public class TypeManager
    {
        readonly List<Type> componentTypes;
        readonly List<Type> tagTypes;

        public TypeManager(params string[] assemblyNames)
        {
            componentTypes = new List<Type>(32);
            tagTypes = new List<Type>(32);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            int assemblyNamesLength = assemblyNames.Length;
            var filtered = assemblies.XIVFilterBy(p =>
            {
                if (assemblyNamesLength == 0) return true; // No name provided, gather all
                var name = p.GetName().Name;
                for (int i = 0; i < assemblyNamesLength; i++)
                {
                    if (name.Contains(assemblyNames[i], StringComparison.InvariantCultureIgnoreCase)) return true;
                }

                return false;
            });

            var componentBase = typeof(IComponent);
            var tagBase = typeof(ITag);

            var filteredLength = filtered.Length;
            for (int i = 0; i < filteredLength; i++)
            {
                var types = filtered[i].GetTypes();
                var typesLength = types.Length;
                for (var j = 0; j < typesLength; j++)
                {
                    var type = types[j];
                    if (type != componentBase && componentBase.IsAssignableFrom(type))
                    {
                        componentTypes.Add(type);
                        continue;
                    }
                    if (type != tagBase && tagBase.IsAssignableFrom(type))
                    {
                        tagTypes.Add(type);
                    }
                }
            }
        }
        
        public IReadOnlyList<Type> GetComponents() => componentTypes;
        public IReadOnlyList<Type> GetTagTypes() => tagTypes;
    }
}