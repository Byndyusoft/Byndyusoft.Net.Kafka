namespace Byndyusoft.Net.Kafka.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static class AssemblyExtensions
    {
        public static IEnumerable<Type> GetTypesAssignableFrom<T>(this Assembly assembly)
        {
            var compareType = typeof(T);

            return assembly.DefinedTypes
                .Where(
                    type =>
                        compareType.IsAssignableFrom(type)
                        && compareType != type
                        && type is {IsClass: true, IsAbstract: false}
                )
                .Cast<Type>()
                .ToArray();
        }

        public static IEnumerable<Assembly> LoadReferencedAssemblies(this Assembly assembly)
        {
            var prefix = assembly.GetName().Name!.Split('.').First();

            var assemblyNames = new HashSet<string>();
            var assembliesStack = new Stack<Assembly>();

            assembliesStack.Push(assembly);

            do
            {
                var currentAssembly = assembliesStack.Pop();

                yield return currentAssembly;

                var referencedAssemblies = currentAssembly.GetReferencedAssemblies()
                    .Where(
                        assemblyName =>
                            string.IsNullOrEmpty(assemblyName.Name) == false
                            && assemblyName.Name.StartsWith(prefix)
                    )
                    .ToArray();

                foreach (var reference in referencedAssemblies)
                    if (assemblyNames.Contains(reference.FullName) == false)
                    {
                        assembliesStack.Push(Assembly.Load(reference));
                        assemblyNames.Add(reference.FullName);
                    }
            } while (assembliesStack.Count > 0);
        }
    }
}