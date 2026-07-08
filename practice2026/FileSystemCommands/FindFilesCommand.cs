using System;
using System.IO;
using CommandLib;

namespace FileSystemCommands
{
    public class FindFilesCommand : ICommand
    {
        private readonly string _directoryPath;
        private readonly string _searchPattern;
        public FindFilesCommand(string directoryPath, string searchPattern)
        {
            _directoryPath = directoryPath ?? throw new ArgumentNullException(nameof(directoryPath));
            _searchPattern = searchPattern ?? throw new ArgumentNullException(nameof(searchPattern));
        }
        [DisplayName("Поиск файлов в каталоге")]
        public void Execute()
        {
            if (!Directory.Exists(_directoryPath))
            {
                Console.WriteLine($"[Ошибка] Каталог не найден: {_directoryPath}");
                return;
            }

            try
            {
                string[] files = Directory.GetFiles(_directoryPath, _searchPattern, SearchOption.AllDirectories);
                Console.WriteLine($"Найдено файлов ({_searchPattern}) в '{_directoryPath}': {files.Length}");

                foreach (var file in files)
                {
                    Console.WriteLine($" - {file}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка] Сбой при поиске файлов: {ex.Message}");
            }
        }
    }
}


