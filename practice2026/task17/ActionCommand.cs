using System;
namespace task17;
public class ActionCommand : ICommand
{
    private readonly Action _action;
    public ActionCommand(Action action) => _action = action;
    public bool Execute()
    {
        _action();
        return true;
    }
}
