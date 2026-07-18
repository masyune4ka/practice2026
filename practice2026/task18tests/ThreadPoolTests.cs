using System;
using System.Collections.Generic;
using System.Threading;
using task17;
using Xunit;

namespace task18tests;

public class ThreadPoolTests
{
    [Fact]
    public void ThreadPool_ExecutesTasks()
    {
        var pool = new task18.ThreadPool();
        bool executed = false;
        pool.Start(2);
        pool.EnqueueTask(new ActionCommand(() => executed = true));
        Thread.Sleep(100);
        pool.Stop();
        Assert.True(executed);
    }
    [Fact]
    public void ThreadPool_WaitsForTasksOnStop()
    {
        var pool = new task18.ThreadPool();
        bool taskFinished = false;
        pool.Start(1);
        pool.EnqueueTask(new ActionCommand(() =>
        {
            Thread.Sleep(200);
            taskFinished = true;
        }));
        pool.Stop();
        Assert.True(taskFinished);
    }
    [Fact]
    public void ThreadPool_ThrowsOnEnqueueAfterStop()
    {
        var pool = new task18.ThreadPool();
        pool.Start(1);
        pool.Stop();
        Assert.Throws<InvalidOperationException>(() => pool.EnqueueTask(new ActionCommand(() => { })));
    }
    [Fact]
    public void ThreadPool_HandlesExceptions()
    {
        var handler = new TestHandler();
        var pool = new task18.ThreadPool(handler);
        var expected = new InvalidOperationException("Test");
        pool.Start(1);
        pool.EnqueueTask(new ActionCommand(() => throw expected));
        Thread.Sleep(100);
        pool.Stop();
        Assert.Same(expected, handler.Caught);
    }
    [Fact]
    public void ThreadPool_SchedulerRoundRobin()
    {
        var pool = new task18.ThreadPool();
        var steps = new List<int>();
        var lockObj = new object();

        pool.Start(1);

        var task1 = new TestCommand(
            () => { lock (lockObj) steps.Add(1); },
            () => { lock (lockObj) return steps.FindAll(x => x == 1).Count == 2; });

        var task2 = new TestCommand(
            () => { lock (lockObj) steps.Add(2); },
            () => { lock (lockObj) return steps.FindAll(x => x == 2).Count == 2; });

        pool.EnqueueTask(task1);
        pool.EnqueueTask(task2);

        Thread.Sleep(500);
        pool.Stop();

        Assert.Equal(4, steps.Count);
        Assert.Equal(2, steps.FindAll(x => x == 1).Count);
        Assert.Equal(2, steps.FindAll(x => x == 2).Count);
    }
    private class TestCommand : ICommand
    {
        private readonly Action _onExecute;
        private readonly Func<bool> _isCompleted;
        public TestCommand(Action onExecute, Func<bool> isCompleted)
        {
            _onExecute = onExecute;
            _isCompleted = isCompleted;
        }
        public bool Execute()
        {
            _onExecute();
            return _isCompleted();
        }
    }
    private class TestHandler : IExceptionHandler
    {
        public Exception Caught { get; private set; } = null!;
        public void Handle(ICommand task, Exception ex)
        {
            Caught = ex;
        }
    }
}
