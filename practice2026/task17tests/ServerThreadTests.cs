using System;
using System.Threading;
using task17;
using Xunit;

namespace task17tests;

public class ServerThreadTests
{
    [Fact]
    public void HardStop_IgnoresRemainingCommands()
    {
        var serverThread = new ServerThread();
        bool cmd1 = false, cmd2 = false;
        serverThread.AddCommand(new ActionCommand(() => cmd1 = true));
        serverThread.AddCommand(new HardStopCommand(serverThread));
        serverThread.AddCommand(new ActionCommand(() => cmd2 = true));
        serverThread.Join(TimeSpan.FromSeconds(2));
        Assert.True(cmd1);
        Assert.False(cmd2);
    }
    [Fact]
    public void SoftStop_ProcessesAllCommands()
    {
        var serverThread = new ServerThread();
        bool cmd1 = false, cmd2 = false;
        serverThread.AddCommand(new ActionCommand(() => cmd1 = true));
        serverThread.AddCommand(new ActionCommand(() => cmd2 = true));
        serverThread.AddCommand(new SoftStopCommand(serverThread));
        serverThread.Join(TimeSpan.FromSeconds(2));
        Assert.True(cmd1);
        Assert.True(cmd2);
    }
    [Fact]
    public void HardStop_ThrowsInWrongThread()
    {
        var serverThread = new ServerThread();
        Assert.Throws<InvalidOperationException>(() => new HardStopCommand(serverThread).Execute());
    }
    [Fact]
    public void SoftStop_ThrowsInWrongThread()
    {
        var serverThread = new ServerThread();
        Assert.Throws<InvalidOperationException>(() => new SoftStopCommand(serverThread).Execute());
    }
    [Fact]
    public void ExceptionHandler_CatchesException()
    {
        var handler = new TestHandler();
        var serverThread = new ServerThread(handler);
        var expected = new InvalidOperationException("Test");
        serverThread.AddCommand(new ActionCommand(() => throw expected));
        serverThread.AddCommand(new SoftStopCommand(serverThread));
        serverThread.Join(TimeSpan.FromSeconds(2));
        Assert.Same(expected, handler.Caught);
    }
    private class TestHandler : IExceptionHandler
    {
        public Exception Caught { get; private set; } = null!;
        public void Handle(ICommand cmd, Exception ex)
        {
            Caught = ex;
        }
    }
}
