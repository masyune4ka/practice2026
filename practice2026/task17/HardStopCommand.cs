using System;
using System.Threading;
namespace task17;
public class HardStopCommand : ICommand
{
    private readonly ServerThread _targetThread;
    public HardStopCommand(ServerThread targetThread) => _targetThread = targetThread;
    public bool Execute()
    {
        if (Thread.CurrentThread != _targetThread.GetThread())
        {
            throw new InvalidOperationException("HardStop только в целевом потоке");
        }
        _targetThread.HardStop();
        return true;
    }
}
