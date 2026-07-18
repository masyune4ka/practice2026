using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new();
    private readonly IExceptionHandler _exceptionHandler;
    private Thread _thread;
    private bool _softStopRequested;
    private bool _hardStopRequested;

    public ServerThread(IExceptionHandler exceptionHandler = null)
    {
        _exceptionHandler = exceptionHandler;
        _thread = new Thread(WorkLoop) { IsBackground = true };
        _thread.Start();
    }

    public Thread GetThread() => _thread;

    public void AddCommand(ICommand command)
    {
        if (_softStopRequested || _hardStopRequested)
        {
            throw new InvalidOperationException("Поток в процессе остановки");
        }
        _queue.Add(command);
    }

    public void HardStop()
    {
        _hardStopRequested = true;
        while (_queue.TryTake(out _)) { }
        _queue.CompleteAdding();
    }

    public void SoftStop()
    {
        _softStopRequested = true;
        _queue.CompleteAdding();
    }

    public void Join(TimeSpan timeout) => _thread.Join(timeout);

    private void WorkLoop()
    {
        foreach (var command in _queue.GetConsumingEnumerable())
        {
            try
            {
                command.Execute();
            }
            catch (Exception ex)
            {
                _exceptionHandler?.Handle(command, ex);
            }

            if (_softStopRequested && _queue.Count == 0)
                break;
        }
    }
}
