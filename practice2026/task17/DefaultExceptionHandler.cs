using System;
namespace task17;
public class DefaultExceptionHandler : IExceptionHandler
{
    public void Handle(ICommand command, Exception exception)
    {
        Console.WriteLine($"Ошибка в {command.GetType().Name}: {exception.Message}");
    }
}
