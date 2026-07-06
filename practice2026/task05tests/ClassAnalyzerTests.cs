using System;
using Xunit;

public class TestClass
{
    public int PublicField;
    private string _privateField;
    public int Property { get; set; }

    public void Method() { }

    public int Calculate(int x, int y) { return x + y; }
}

[Serializable]
public class AttributedClass { }

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods();

        Assert.Contains("Method", methods);
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields();

        Assert.Contains("_privateField", fields);
    }

    [Fact]
    public void GetMethodParams_ReturnsParametersAndReturnType()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var result = analyzer.GetMethodParams("Calculate");

        Assert.Contains("x", result);
        Assert.Contains("y", result);
        Assert.Contains("Returns: Int32", result);
    }

    [Fact]
    public void GetProperties_ReturnsPropertyNames()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var properties = analyzer.GetProperties();

        Assert.Contains("Property", properties);
    }

    [Fact]
    public void HasAttribute_ReturnsTrueForAttributedClass()
    {
        var analyzer = new ClassAnalyzer(typeof(AttributedClass));
        var result = analyzer.HasAttribute<SerializableAttribute>();

        Assert.True(result);
    }

    [Fact]
    public void HasAttribute_ReturnsFalseForNonAttributedClass()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var result = analyzer.HasAttribute<SerializableAttribute>();

        Assert.False(result);
    }
}
