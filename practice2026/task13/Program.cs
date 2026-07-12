using System;
using System.IO;
using task13;

namespace task13;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Сериализация и десериализация Student ===\n");
        // Создаём студента
        var student = new Student
        {
            FirstName = "Иван",
            LastName = "Петров",
            BirthDate = new DateTime(2005, 5, 15),
            MiddleName = "Иванович",
            Email = null,  // null-значение будет проигнорировано
            Grades = new List<Subject>
            {
                new Subject { Name = "Математика", Grade = 5 },
                new Subject { Name = "Физика", Grade = 4 },
                new Subject { Name = "Английский", Grade = 5 }
            }
        };
        Console.WriteLine("1. Исходный объект:");
        Console.WriteLine($"   {student.FirstName} {student.LastName}");
        Console.WriteLine($"   Дата рождения: {student.BirthDate:dd.MM.yyyy}");
        Console.WriteLine($"   Оценок: {student.Grades.Count}\n");

        // Сериализация
        var helper = new JsonHelper();
        string json = helper.Serialize(student);

        Console.WriteLine("2. JSON (сериализация):");
        Console.WriteLine(json);
        Console.WriteLine();

        // Сохранение в файл
        string filePath = "student.json";
        helper.SaveToFile(student, filePath);
        Console.WriteLine($"3. JSON сохранён в файл: {Path.GetFullPath(filePath)}\n");

        // Загрузка из файла
        Console.WriteLine("4. Загрузка из файла:");
        string loadedJson = File.ReadAllText(filePath);
        Console.WriteLine(loadedJson);
        Console.WriteLine();

        // Десериализация
        var loadedStudent = helper.LoadFromFile(filePath);

        Console.WriteLine("5. Десериализованный объект:");
        Console.WriteLine($"   {loadedStudent.FirstName} {loadedStudent.LastName}");
        Console.WriteLine($"   Дата рождения: {loadedStudent.BirthDate:dd.MM.yyyy}");
        Console.WriteLine($"   Оценок: {loadedStudent.Grades.Count}");
        Console.WriteLine("   Предметы:");
        foreach (var subject in loadedStudent.Grades)
        {
            Console.WriteLine($"     - {subject.Name}: {subject.Grade}");
        }
        Console.WriteLine("\n=== Готово! ===");
        Console.WriteLine("Нажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}
