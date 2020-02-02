using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LogsAnalyzer.Infrastructure.Factory  {
    internal class TypeFactory {
        private static Dictionary<string, Assembly> _loadedAssemblies = new Dictionary<string, Assembly>();

        public static bool DoesTypeExistInAssembly(Assembly assembly, string typeName) {
            return assembly.GetTypes().Any(t => t.FullName == typeName);
        }

        public static Assembly ResolveAssembly(string assemblyName) {
            if (_loadedAssemblies.ContainsKey(assemblyName)) {
                return _loadedAssemblies[assemblyName];
            }
            else {
                Assembly asm = null;
                try {
                    asm = Assembly.Load(assemblyName);
                }
                catch (Exception exc) {
                    throw new AssemblyLoadFailedException(assemblyName, exc.Message);
                }

                _loadedAssemblies.Add(assemblyName, asm);
                return asm;
            }
        }

        public static T CreateInstance<T>(FullTypeNameEntry fullTypeNameEntry, object[] constructorArgs) where T : class {
            return CreateInstance<T>(fullTypeNameEntry.AssemblyName, fullTypeNameEntry.TypeName, constructorArgs);
        }

        public static T CreateInstance<T>(string assemblyName, string typeName, object[] constructorArgs) where T : class {
            var asm = ResolveAssembly(assemblyName);

            T targetObject = null;
            try {
                targetObject = asm.CreateInstance(typeName, true, BindingFlags.CreateInstance, null,
                               constructorArgs, null, null) as T;
            }
            catch (Exception exc) {
                throw new TypeLoadFailedException(typeName, assemblyName, exc.Message);
            }

            if (targetObject == null) throw new TypeLoadFailedException(typeName, assemblyName);

            return targetObject;
        }

        public static object CreateInstance(string assemblyName, string typeName, object[] constructorArgs) {
            var asm = ResolveAssembly(assemblyName);

            var targetObject = asm.CreateInstance(typeName, true, BindingFlags.CreateInstance, null,
                            constructorArgs, null, null);
            if (targetObject == null) throw new TypeLoadFailedException(typeName, assemblyName);

            return targetObject;
        }

    }

    public struct FullTypeNameEntry {
        public readonly string AssemblyName;
        public readonly string TypeName;

        public FullTypeNameEntry(string fullTypeName) {
            if (string.IsNullOrWhiteSpace(fullTypeName)) throw new MissingTypeNameException();

            var nameParts = fullTypeName.Split(',');
            if (nameParts.Length != 2) throw new InvalidTypeNameFormatException(fullTypeName, nameParts.Length);

            AssemblyName = nameParts[0].Trim();
            TypeName = nameParts[1].Trim();
        }
    }

    public class MissingTypeNameException : ArgumentException {
        public MissingTypeNameException() 
            : base("TypeName should not be null or empty"){

        }
    }

    public class InvalidTypeNameFormatException : ArgumentException {
        public InvalidTypeNameFormatException(string typeName, int segmentCount)
              : base($"Expected segment for {typeName} is 2 but actual is {segmentCount}") {
        }
    }

    public class TypeLoadFailedException : ArgumentException {
        public TypeLoadFailedException(string typeName, string assemblyName)
            : base($"Type {typeName} not found in assembly {assemblyName}") {

        }
        public TypeLoadFailedException(string typeName, string assemblyName, string message)
           : base($"Type {typeName} not found in assembly {assemblyName}. Details: {message}") {

        }
    }

    public class AssemblyLoadFailedException : ArgumentException {
        public AssemblyLoadFailedException(string assemblyName, string errorDetails)
            : base($"Assembly {assemblyName} not found. Details {errorDetails}") {

        }
    }
}
