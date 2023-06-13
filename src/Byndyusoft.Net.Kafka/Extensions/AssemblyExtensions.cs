namespace Byndyusoft.Net.Kafka.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    
    internal static class AssemblyExtensions
    {
        public static List<Type> GetTypesAssignableFrom<T>(this Assembly assembly)
        {
            var compareType = typeof(T);
            return assembly.DefinedTypes
                .Where(type => 
                    compareType.IsAssignableFrom(type) 
                    && compareType != type
                    && type.IsClass
                    && type.IsAbstract == false
                )
                .Cast<Type>()
                .ToList();
        }

        public static IEnumerable<Assembly> LoadReferencedAssemblies(this Assembly assembly, 
            Func<AssemblyName, bool> assemblyNamePredicate)
        {
            var assemblyNames = new HashSet<string>();
            var assembliesStack = new Stack<Assembly>();

            assembliesStack.Push(assembly);

            do
            {
                var currentAssembly = assembliesStack.Pop();

                yield return currentAssembly;
                
                var referencedAssemblies = currentAssembly.GetReferencedAssemblies()
                    .Where(assemblyName => string.IsNullOrEmpty(assemblyName.Name) == false 
                                           && assemblyNames.Contains(assemblyName.FullName) == false
                                           && assemblyNamePredicate(assemblyName))
                    .ToArray();

                foreach (var reference in referencedAssemblies)
                {
                    assembliesStack.Push(Assembly.Load(reference)); 
                    assemblyNames.Add(reference.FullName);
                }
                    
            } while (assembliesStack.Count > 0);
        }
    }
}