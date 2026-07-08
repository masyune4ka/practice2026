using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MetadataViewer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("[Ошибка] Укажите путь к DLL-файлу в параметрах командной строки.");
                return;
            }

            string dllPath = args[0];

            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"[Ошибка] Файл не найден: {dllPath}");
                return;
            }
            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);
                Console.WriteLine($"Метаданные библиотеки: {assembly.GetName().Name}\n");

                var classes = assembly.GetTypes().Where(t => t.IsClass && !t.Name.StartsWith("<"));

                foreach (var type in classes)
                {
                    Console.WriteLine($"Класс: {type.FullName}");

                    var classAttributes = type.GetCustomAttributes();
                    if (classAttributes.Any())
                    {
                        Console.WriteLine("  [Атрибуты]:");
                        foreach (var attr in classAttributes)
                        {
                            Console.WriteLine($"    - {attr.GetType().Name}");
                        }
                    }
                    var constructors = type.GetConstructors();
                    if (constructors.Any())
                    {
                        Console.WriteLine("  [Конструкторы]:");
                        foreach (var ctor in constructors)
                        {
                            var parameters = ctor.GetParameters();
                            string paramsStr = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
                            Console.WriteLine($"    - {type.Name}({paramsStr})");
                        }
                    }
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    if (methods.Any())
                    {
                        Console.WriteLine("  [Методы]:");
                        foreach (var method in methods)
                        {
                            if (method.IsSpecialName) continue;

                            var parameters = method.GetParameters();
                            string paramsStr = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
                            Console.WriteLine($"    - {method.ReturnType.Name} {method.Name}({paramsStr})");
                        }
                    }
                    Console.WriteLine(new string('-', 50));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка] Не удалось прочитать метаданные: {ex.Message}");
            }
        }
    }
}

