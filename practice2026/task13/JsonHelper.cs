using System;
using System.IO;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13;

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private const string DateFormat = "dd.MM.yyyy";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();

        if (DateTime.TryParseExact(dateString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return date;

        throw new JsonException($"Неверный формат даты: {dateString}. Ожидается: {DateFormat}");
    }
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}
public class JsonHelper
{
    private JsonSerializerOptions Options;

    public JsonHelper()
    {
        Options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        Options.Converters.Add(new CustomDateTimeConverter());
    }

    public string Serialize(Student student)
    {
        if (student == null)
            throw new ArgumentNullException(nameof(student), "Студент не может быть null");

        return JsonSerializer.Serialize(student, Options);
    }
    public Student Deserialize(string json)
    {
        if (string.IsNullOrEmpty(json))
            throw new ArgumentException("JSON не может быть пустым", nameof(json));

        var student = JsonSerializer.Deserialize<Student>(json, Options);

        if (student == null)
            throw new InvalidOperationException("Не удалось десериализовать JSON");

        ValidateStudent(student);

        return student;
    }
    public void SaveToFile(Student student, string filePath)
    {
        if (student == null)
            throw new ArgumentNullException(nameof(student), "Студент не может быть null");

        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("Путь к файлу не может быть пустым", nameof(filePath));

        string json = Serialize(student);
        File.WriteAllText(filePath, json);
    }
    public Student LoadFromFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("Путь к файлу не может быть пустым", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException("Файл не найден", filePath);

        string json = File.ReadAllText(filePath);
        return Deserialize(json);
    }
    private void ValidateStudent(Student student)
    {
        if (string.IsNullOrEmpty(student.FirstName))
            throw new InvalidOperationException("Имя обязательно");

        if (string.IsNullOrEmpty(student.LastName))
            throw new InvalidOperationException("Фамилия обязательна");

        if (student.BirthDate > DateTime.Today)
            throw new InvalidOperationException("Дата рождения не может быть в будущем");

        if (student.Grades == null)
            throw new InvalidOperationException("Список оценок не может быть null");

        foreach (var subject in student.Grades)
        {
            if (string.IsNullOrEmpty(subject.Name))
                throw new InvalidOperationException("Название предмета обязательно");

            if (subject.Grade < 2 || subject.Grade > 5)
                throw new InvalidOperationException($"Оценка должна быть от 2 до 5, получено: {subject.Grade}");
        }
    }
}
