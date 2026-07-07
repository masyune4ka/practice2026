using System;
using System.IO;
using CommandLib;

namespace FileSystemCommands
{
    public class DirectorySizeCommand : ICommand
    {
        private readonly string _directoryPath;

        public DirectorySizeCommand(string directoryPath)
        {
            _directoryPath = directoryPath ?? throw new ArgumentNullException(nameof(directoryPath));
        }
        public void Execute()
        {
            if (!Directory.Exists(_directoryPath))
            {
                Console.WriteLine($"[Ошибка] Каталог не найден: {_directoryPath}");
                return;
            }
            try
            {
                long size = CalculateDirectorySize(new DirectoryInfo(_directoryPath));
                Console.WriteLine($"Размер каталога '{_directoryPath}': {size} байт");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка] Не удалось вычислить размер: {ex.Message}");
            }
        }
        private long CalculateDirectorySize(DirectoryInfo dir)
        {
            long size = 0;
            foreach (FileInfo file in dir.GetFiles())
            {
                size += file.Length;
            }
            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                size += CalculateDirectorySize(subDir);
            }
            return size;
        }
    }
}
