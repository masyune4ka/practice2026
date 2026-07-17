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
    private readonly object _lock = new();

    public ServerThread(IExceptionHandler exceptionHandler = null)
    {
        _exceptionHandler = exceptionHandler;
        _thread = new Thread(WorkLoop) { IsBackground = true };
        _thread.Start();
    }
    public Thread GetThread() => _thread;

    public void AddCommand(ICommand command)
    {
        lock (_lock)
        {
            if (_softStopRequested)
            {
                throw new InvalidOperationException("Поток в процессе SoftStop");
            }
            _queue.Add(command);
            Monitor.Pulse(_lock);
        }
    }
    public void HardStop()
    {
        lock (_lock)
        {
            _hardStopRequested = true;
            _queue.CompleteAdding();
            Monitor.Pulse(_lock);
        }
    }

    public void SoftStop()
    {
        lock (_lock)
        {
            _softStopRequested = true;
            Monitor.Pulse(_lock);
        }
    }
    public void Join(TimeSpan timeout) => _thread.Join(timeout);
    private void WorkLoop()
    {
        while (true)
        {
            ICommand command = null;
            lock (_lock)
            {
                while (_queue.Count == 0 && !_queue.IsCompleted && !_softStopRequested)
                {
                    Monitor.Wait(_lock);
                }
                if (_queue.IsCompleted || _hardStopRequested)
                {
                    break;
                }
                if (_softStopRequested && _queue.Count == 0)
                {
                    break;
                }
                if (_queue.Count > 0)
                {
                    command = _queue.Take();
                }
            }
            if (command != null)
            {
                try
                {
                    command.Execute();
                }
                catch (Exception ex)
                {
                    _exceptionHandler?.Handle(command, ex);
                }
            }
        }
    }
}
