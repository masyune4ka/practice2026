using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using pluginload;

namespace task10
{
    public class PluginLoader
    {
        private readonly string _pluginsPath;
        public PluginLoader(string pluginsPath)
        {
            _pluginsPath = pluginsPath;
        }
        public void LoadAndExecutePlugins()
        {
            var plugins = LoadPlugins();
            var sortedPlugins = TopologicalSort(plugins);
            ExecutePlugins(sortedPlugins);
        }
        private List<IPlugin> LoadPlugins()
        {
            var plugins = new List<IPlugin>();
            var dllFiles = Directory.GetFiles(_pluginsPath, "*.dll");

            foreach (var dllFile in dllFiles)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dllFile);
                    var pluginTypes = FindPluginTypes(assembly);

                    foreach (var pluginType in pluginTypes)
                    {
                        var plugin = Activator.CreateInstance(pluginType) as IPlugin;
                        if (plugin != null)
                        {
                            plugins.Add(plugin);
                        }
                    }
                }
                catch
                {
                    // Пропускаем файлы, которые не являются плагинами
                }
            }

            return plugins;
        }
        private List<Type> FindPluginTypes(Assembly assembly)
        {
            var pluginTypes = new List<Type>();

            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.GetCustomAttribute<PluginLoadAttribute>() != null &&
                        typeof(IPlugin).IsAssignableFrom(type) &&
                        !type.IsAbstract)
                    {
                        pluginTypes.Add(type);
                    }
                }
            }
            catch (ReflectionTypeLoadException)
            {
                // Пропускаем сборки с ошибками загрузки типов
            }

            return pluginTypes;
        }
        private List<IPlugin> TopologicalSort(List<IPlugin> plugins)
        {
            var pluginDict = new Dictionary<string, IPlugin>();
            var dependencies = new Dictionary<string, List<string>>();

            foreach (var plugin in plugins)
            {
                var type = plugin.GetType();
                var attribute = type.GetCustomAttribute<PluginLoadAttribute>();
                var deps = attribute?.Dependencies.ToList() ?? new List<string>();

                pluginDict[plugin.Name] = plugin;
                dependencies[plugin.Name] = deps;
            }

            var visited = new HashSet<string>();
            var result = new List<IPlugin>();

            void Visit(string pluginName)
            {
                if (visited.Contains(pluginName))
                {
                    return;
                }

                visited.Add(pluginName);

                if (dependencies.ContainsKey(pluginName))
                {
                    foreach (var dep in dependencies[pluginName])
                    {
                        if (pluginDict.ContainsKey(dep))
                        {
                            Visit(dep);
                        }
                    }
                }
                if (pluginDict.ContainsKey(pluginName))
                {
                    result.Add(pluginDict[pluginName]);
                }
            }
            foreach (var pluginName in pluginDict.Keys)
            {
                Visit(pluginName);
            }
            return result;
        }
        private void ExecutePlugins(List<IPlugin> plugins)
        {
            foreach (var plugin in plugins)
            {
                plugin.Execute();
            }
        }
    }
}
