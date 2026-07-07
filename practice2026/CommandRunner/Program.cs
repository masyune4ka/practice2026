using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLib;

namespace CommandRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileSystemCommands.dll");

            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"[Ошибка] Файл не найден: {dllPath}");
                return;
            }
            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);
                Console.WriteLine($"[Успех] Сборка {assembly.GetName().Name} загружена.");

                var commandTypes = assembly.GetTypes()
                    .Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                    .ToList();

                string testDirectory = Directory.GetCurrentDirectory();

                foreach (var type in commandTypes)
                {
                    Console.WriteLine($" Выполнение {type.Name}");
                    ICommand command = null;

                    if (type.Name == "DirectorySizeCommand")
                    {
                        command = (ICommand)Activator.CreateInstance(type, testDirectory);
                    }
                    else if (type.Name == "FindFilesCommand")
                    {
                        command = (ICommand)Activator.CreateInstance(type, testDirectory, "*.*");
                    }

                    if (command != null)
                    {
                        command.Execute();
                    }
                    else
                    {
                        Console.WriteLine($"[Предупреждение] Не удалось создать экземпляр {type.Name}");
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Критическая ошибка] {ex.Message}");
            }
        }
    }
}
