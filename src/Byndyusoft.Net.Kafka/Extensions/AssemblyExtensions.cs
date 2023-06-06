using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Byndyusoft.Net.Kafka.Extensions
{
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

        public static IEnumerable<Assembly> LoadReferencedAssemblies(this Assembly assembly)
        {
            var assemblyNames = new HashSet<string>();
            var stack = new Stack<Assembly>();

            stack.Push(assembly);

            do
            {
                var asm = stack.Pop();

                yield return asm;

                var referencedAssemblies = asm.GetReferencedAssemblies()
                    .Where(a => string.IsNullOrEmpty(a.Name) == false)
                    .ToArray();
                foreach (var reference in referencedAssemblies)
                    if (assemblyNames.Contains(reference.FullName) == false)
                    {
                        stack.Push(Assembly.Load(reference));
                        assemblyNames.Add(reference.FullName);
                    }

            }
            while (stack.Count > 0);
        }
    }
}