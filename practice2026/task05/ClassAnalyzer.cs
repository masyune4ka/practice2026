using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

public class ClassAnalyzer
{
    private Type _type;

    public ClassAnalyzer(Type type)
    {
        _type = type;
    }
    public IEnumerable<string> GetPublicMethods()
    {
        return _type.GetMethods().Select(m => m.Name);
    }
    public IEnumerable<string> GetMethodParams(string methodname)
    {
        var method = _type.GetMethod(methodname);

        if (method == null)
        {
            return Enumerable.Empty<string>();
        }
        var parameters = method.GetParameters().Select(p => p.Name);
        var returnType = $"Returns: {method.ReturnType.Name}";
        return parameters.Append(returnType);
    }
    public IEnumerable<string> GetAllFields()
    {
        var fields = _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        return fields.Select(f => f.Name);
    }
    public IEnumerable<string> GetProperties()
    {
        return _type.GetProperties().Select(p => p.Name);
    }
    public bool HasAttribute<T>()
    {
        var attributes = _type.GetCustomAttributes(typeof(T), false);

        if (attributes.Length > 0)
        {
            return true;
        }
        return false;

    }
}

