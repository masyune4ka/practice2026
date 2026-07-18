using System.Threading;
using task17;

namespace task19;

public class TestCommand : ICommand
{
    private readonly int _id;
    private readonly ServerThread _serverThread;
    private readonly int _maxCalls;
    private int _counter;

    public TestCommand(int id, ServerThread serverThread, int maxCalls = 3)
    {
        _id = id;
        _serverThread = serverThread;
        _maxCalls = maxCalls;
        _counter = 0;
    }
    public void Execute()
    {
        _counter++;
        Console.WriteLine($"Команда {_id}: вызов {_counter} из {_maxCalls}");
        Thread.Sleep(10);

        if (_counter < _maxCalls)
        {
            _serverThread.AddCommand(this);
        }
        else
        {
            Interlocked.Decrement(ref Program._activeCommands);
        }
    }
}
public class CompletionCommand : ICommand
{
    private readonly ServerThread _serverThread;

    public CompletionCommand(ServerThread serverThread)
    {
        _serverThread = serverThread;
    }

    public void Execute()
    {
        if (Program._activeCommands > 0)
        {
            _serverThread.AddCommand(this);
        }
        else
        {
            _serverThread.HardStop();
        }
    }
}
