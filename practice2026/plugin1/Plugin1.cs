using pluginload;

namespace plugin1
{
    [PluginLoad]
    public class Plugin1 : IPlugin
    {
        public string Name => "Plugin1";
        public void Execute()
        {
            Console.WriteLine("Executing Plugin1");
        }
    }
}
