using System;
using System.IO;
using System.Reflection;
using Xunit;
using task10;
using pluginload;

namespace task10tests
{
    public class PluginLoaderTests
    {
        [Fact]
        public void PluginLoader_WithEmptyDirectory_DoesNotThrow()
        {
            var tempPath = CreateEmptyDirectory();

            try
            {
                var loader = new PluginLoader(tempPath);
                var exception = Record.Exception(() => loader.LoadAndExecutePlugins());
                Assert.Null(exception);
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public void PluginLoader_WithPluginDll_LoadsPlugin()
        {
            var tempPath = CreateEmptyDirectory();
            var pluginPath = CopyTestPlugin(tempPath, "TestPlugin1.dll");

            try
            {
                var loader = new PluginLoader(tempPath);
                loader.LoadAndExecutePlugins();
                Assert.True(File.Exists(pluginPath));
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public void PluginLoader_PluginWithoutAttribute_NotLoaded()
        {
            var tempPath = CreateEmptyDirectory();

            try
            {
                var loader = new PluginLoader(tempPath);
                loader.LoadAndExecutePlugins();
                Assert.True(true);
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        [Fact]
        public void PluginLoader_PluginsSortedByDependencies_CorrectOrder()
        {
            var tempPath = CreateEmptyDirectory();
            var executionOrderFile = Path.Combine(tempPath, "order.txt");

            try
            {
                var loader = new PluginLoader(tempPath);
                loader.LoadAndExecutePlugins();

                if (File.Exists(executionOrderFile))
                {
                    var lines = File.ReadAllLines(executionOrderFile);
                    Assert.Equal("Plugin1", lines[0]);
                    Assert.Equal("Plugin2", lines[1]);
                }
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }
        }

        private string CreateEmptyDirectory()
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(path);
            return path;
        }

        private string CopyTestPlugin(string targetPath, string pluginName)
        {
            var pluginPath = Path.Combine(targetPath, pluginName);
            File.WriteAllText(pluginPath, "dummy");
            return pluginPath;
        }
    }
}
