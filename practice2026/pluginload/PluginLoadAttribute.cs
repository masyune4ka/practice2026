using System;

namespace pluginload
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginLoadAttribute : Attribute
    {
        public string[] Dependencies { get; }
        public PluginLoadAttribute(params string[] dependencies)
        {
            Dependencies = dependencies ?? Array.Empty<string>();
        }
    }
}