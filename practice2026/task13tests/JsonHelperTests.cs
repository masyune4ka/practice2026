using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using task13;
using Xunit;

namespace task13tests;

public class JsonSerializationTests
{
    private JsonHelper helper = new JsonHelper();

    [Fact]
    public void Serialize_ValidStudent_ReturnsValidJson()
    {
        var student = new Student
        {
            FirstName = "Иван",
            LastName = "Петров",
            BirthDate = new DateTime(2005, 5, 15),
            Grades = new List<Subject>
            {
                new Subject { Name = "Математика", Grade = 5 },
                new Subject { Name = "Английский", Grade = 4 }
            }
        };

        var json = helper.Serialize(student);

        Assert.NotNull(json);
        Assert.Contains("first_name", json);
        Assert.Contains("15.05.2005", json);
    }

    [Fact]
    public void Serialize_NullEmail_NotIncludedInJson()
    {
        var student = new Student
        {
            FirstName = "Анна",
            LastName = "Смирнова",
            BirthDate = new DateTime(2004, 10, 20),
            Email = null,
            Grades = new List<Subject>()
        };

        var json = helper.Serialize(student);

        Assert.NotNull(json);
        Assert.DoesNotContain("email", json.ToLower());
    }

    [Fact]
    public void Deserialize_ValidJson_ReturnsStudentObject()
    {
        var json = @"{
            ""first_name"": ""Maria"",
            ""last_name"": ""Ivanova"",
            ""birth_date"": ""10.03.2006"",
            ""grades"": [
                { ""name"": ""Physics"", ""grade"": 5 }
            ]
        }";

        var student = helper.Deserialize(json);

        Assert.Equal("Maria", student.FirstName);
        Assert.Equal(new DateTime(2006, 3, 10), student.BirthDate);
        Assert.Single(student.Grades);
    }

    [Fact]
    public void Deserialize_InvalidDateFormat_ThrowsException()
    {
        var json = @"{
            ""first_name"": ""Boris"",
            ""last_name"": ""Borisov"",
            ""birth_date"": ""2006-03-10"",
            ""grades"": []
        }";

        Assert.Throws<JsonException>(() => helper.Deserialize(json));
    }

    [Fact]
    public void Deserialize_InvalidGrade_ThrowsException()
    {
        var json = @"{
            ""first_name"": ""Charlie"",
            ""last_name"": ""Chaplin"",
            ""birth_date"": ""15.07.2005"",
            ""grades"": [
                { ""name"": ""Chemistry"", ""grade"": 10 }
            ]
        }";

        Assert.Throws<InvalidOperationException>(() => helper.Deserialize(json));
    }

    [Fact]
    public void SaveToFile_CreatesFileWithValidJson()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.json");
        var student = new Student
        {
            FirstName = "Elena",
            LastName = "Kuznetsova",
            BirthDate = new DateTime(2005, 8, 25),
            Grades = new List<Subject> { new Subject { Name = "Biology", Grade = 4 } }
        };

        try
        {
            helper.SaveToFile(student, filePath);

            Assert.True(File.Exists(filePath));
            var content = File.ReadAllText(filePath);
            Assert.Contains("Elena", content);
            Assert.Contains("Biology", content);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public void LoadFromFile_ReturnsStudentObject()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.json");
        var expected = new Student
        {
            FirstName = "Michael",
            LastName = "Anderson",
            BirthDate = new DateTime(2006, 1, 15),
            Grades = new List<Subject> { new Subject { Name = "History", Grade = 5 } }
        };

        try
        {
            helper.SaveToFile(expected, filePath);
            var loaded = helper.LoadFromFile(filePath);

            Assert.Equal(expected.FirstName, loaded.FirstName);
            Assert.Equal(expected.BirthDate, loaded.BirthDate);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public void LoadFromFile_FileNotFound_ThrowsException()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"nonexistent_{Guid.NewGuid()}.json");

        Assert.Throws<FileNotFoundException>(() => helper.LoadFromFile(filePath));
    }

    [Fact]
    public void Serialize_NullStudent_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => helper.Serialize(null!));
    }

    [Fact]
    public void Deserialize_EmptyJson_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => helper.Deserialize(string.Empty));
    }
}
