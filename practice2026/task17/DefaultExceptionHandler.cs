using System;

namespace task17;

public class DefaultExceptionHandler : IExceptionHandler
{
    public void Handle(ICommand command, Exception exception)
    {
        Console.WriteLine($"[ExceptionHandler] Ошибка в {command.GetType().Name}: {exception.Message}");
    }
}
