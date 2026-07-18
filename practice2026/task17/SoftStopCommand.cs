using System;
using System.Threading;
namespace task17;
public class SoftStopCommand : ICommand
{
    private readonly ServerThread _targetThread;
    public SoftStopCommand(ServerThread targetThread) => _targetThread = targetThread;
    public bool Execute()
    {
        if (Thread.CurrentThread != _targetThread.GetThread())
        {
            throw new InvalidOperationException("SoftStop только в целевом потоке");
        }
        _targetThread.SoftStop();
        return true;
    }
}
