using System;
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
        int executionOrder = 0;
        int task1Step = 0;
        int task2Step = 0;
        pool.Start(1);
        pool.EnqueueTask(new TestCommand(() => task1Step = ++executionOrder, () => task1Step == 2));
        pool.EnqueueTask(new TestCommand(() => task2Step = ++executionOrder, () => task2Step == 2));
        Thread.Sleep(300);
        pool.Stop();
        Assert.Equal(2, task1Step);
        Assert.Equal(2, task2Step);
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
