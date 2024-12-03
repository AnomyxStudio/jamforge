using System;
using System.Collections.Generic;
using System.Reflection;

namespace JamForge
{
    public static class TypeFinder
    {
        private static readonly Dictionary<string, Type> TypeCaches = new();

        private static readonly List<Assembly> UnderlyingAssemblies = new();

        private static List<Assembly> Assemblies
        {
            get
            {
                if (UnderlyingAssemblies.Count == 0)
                {
                    UnderlyingAssemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
                }

                return UnderlyingAssemblies;
            }
        }

        private static Type GetTypeInAllAssemblies(string typeName)
        {
            for (var i = 0; i < Assemblies.Count; i++)
            {
                var assembly = Assemblies[i];
                var type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        public static Type Get(string typeName)
        {
            // Check if it's cached
            if (TypeCaches.TryGetValue(typeName, out var type))
            {
                return type;
            }

            // Check if it's in the current assembly
            type = Type.GetType(typeName);
            if (type != null)
            {
                TypeCaches.Add(typeName, type);
                return type;
            }

            // Check if it's in all assemblies
            type = GetTypeInAllAssemblies(typeName);
            if (type != null)
            {
                TypeCaches.Add(typeName, type);
                return type;
            }

            // Not a valid type
            return null;
        }

        public static List<Type> GetSubclassesOf(Type baseType, bool includeAbstract = false)
        {
            var types = new List<Type>();
            for (var i = 0; i < Assemblies.Count; i++)
            {
                var assembly = Assemblies[i];
                var assemblyTypes = assembly.GetTypes();
                for (var j = 0; j < assemblyTypes.Length; j++)
                {
                    var type = assemblyTypes[j];
                    if (type.IsSubclassOf(baseType) && (includeAbstract || !type.IsAbstract))
                    {
                        types.Add(type);
                    }
                }
            }

            return types;
        }
    }
}
