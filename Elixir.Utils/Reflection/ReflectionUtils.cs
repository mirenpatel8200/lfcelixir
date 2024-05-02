using System;
using System.IO;
using System.Reflection;

namespace Elixir.Utils.Reflection
{
    public static class ReflectionUtils
    {
        public static void ClonePublicProperties<TSource, TDest>(TSource sourceObject, TDest destObject)
        {
            if (sourceObject == null)
                throw new ArgumentNullException(nameof(sourceObject));
            if (destObject == null)
                throw new ArgumentNullException(nameof(destObject));

            PropertyInfo[] sourceProperties = typeof(TSource).GetProperties();
            PropertyInfo[] destProperties = typeof(TDest).GetProperties();

            foreach (PropertyInfo sourceProperty in sourceProperties)
            {
                String sourcePropName = sourceProperty.Name;

                foreach (PropertyInfo destProperty in destProperties)
                {
                    String destPropName = destProperty.Name;

                    if (sourcePropName.Equals(destPropName) && sourceProperty.GetMethod.IsPublic && destProperty.GetMethod.IsPublic && destProperty.GetSetMethod() != null)
                    {
                        destProperty.SetValue(destObject, sourceProperty.GetValue(sourceObject));
                    }
                }
            }
        }

        public static string GetAssemblyDirectory(this Assembly asm)
        {
            var codeBase = asm.CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);

            return Path.GetDirectoryName(path);
        }
    }
}
