using System;
using System.Collections.Generic;
using System.Threading;
using task17;

namespace task18;
public class ThreadPool
{
    private readonly Queue<ICommand> _queue = new();
    private readonly Queue<ICommand> _scheduler = new();
    private readonly IExceptionHandler _exceptionHandler;
    private List<Thread> _threads;
    private bool _isStopped;
    private readonly object _lock = new();
    public ThreadPool(IExceptionHandler exceptionHandler = null)
    {
        _exceptionHandler = exceptionHandler;
        _isStopped = false;
    }
    public void Start(int threadsCount)
    {
        _threads = new List<Thread>();
        for (int i = 0; i < threadsCount; i++)
        {
            var thread = new Thread(WorkerLoop) { IsBackground = true };
            _threads.Add(thread);
            thread.Start();
        }
    }
    public void EnqueueTask(ICommand task)
    {
        lock (_lock)
        {
            if (_isStopped)
            {
                throw new InvalidOperationException("Пул потоков остановлен");
            }
            _queue.Enqueue(task);
            Monitor.Pulse(_lock);
        }
    }
    public void Stop()
    {
        lock (_lock)
        {
            _isStopped = true;
            Monitor.PulseAll(_lock);
        }

        if (_threads != null)
        {
            foreach (var thread in _threads)
            {
                thread.Join();
            }
        }
    }
    private void WorkerLoop()
    {
        while (true)
        {
            ICommand task = null;
            lock (_lock)
            {
                while (_queue.Count == 0 && _scheduler.Count == 0 && !_isStopped)
                {
                    Monitor.Wait(_lock);
                }
                if (_isStopped && _queue.Count == 0 && _scheduler.Count == 0)
                {
                    return;
                }
                if (_scheduler.Count > 0)
                {
                    task = _scheduler.Dequeue();
                }
                else if (_queue.Count > 0)
                {
                    task = _queue.Dequeue();
                }
            }
            if (task != null)
            {
                try
                {
                    bool isCompleted = task.Execute();
                    if (!isCompleted)
                    {
                        lock (_lock)
                        {
                            _scheduler.Enqueue(task);
                            Monitor.Pulse(_lock);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _exceptionHandler?.Handle(task, ex);
                }
            }
        }
    }
}
