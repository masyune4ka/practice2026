using pluginload;

namespace plugin2
{
    [PluginLoad("Plugin1")]
    public class Plugin2 : IPlugin
    {
        public string Name => "Plugin2";
        public void Execute()
        {
            Console.WriteLine("Executing Plugin2");
        }
    }
}
