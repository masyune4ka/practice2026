using System;
using System.Reflection;

namespace task07
{
    public static class ReflectionHelper
    {
        public static void PrintTypeInfo(Type type)
        {
            if (type == null) return;

            var classDisplayNameAttr = type.GetCustomAttribute<DisplayNameAttribute>();
            if (classDisplayNameAttr != null)
            {
                Console.WriteLine($"Класс: {classDisplayNameAttr.DisplayName}");
            }
            var versionAttr = type.GetCustomAttribute<VersionAttribute>();
            if (versionAttr != null)
            {
                Console.WriteLine($"Версия: {versionAttr}");
            }
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            foreach (var prop in properties)
            {
                var propAttr = prop.GetCustomAttribute<DisplayNameAttribute>();
                if (propAttr != null)
                {
                    Console.WriteLine($"Свойство: {prop.Name} ({propAttr.DisplayName})");
                }
            }
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                var methodAttr = method.GetCustomAttribute<DisplayNameAttribute>();
                if (methodAttr != null)
                {
                    Console.WriteLine($"Метод: {method.Name} ({methodAttr.DisplayName})");
                }
            }
        }
    }
}
