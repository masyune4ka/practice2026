using System;
using System.Diagnostics;
using System.Threading;

namespace task14;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Вычисление интеграла методом трапеций ===\n");

        var X = (double x) => x;
        Console.WriteLine("Интеграл от x на [-1, 1]:");
        double result1 = DefiniteIntegral.Solve(-1, 1, X, 1e-4, 4);
        Console.WriteLine($"Результат: {result1:F6} (ожидаемо: 0.0)\n");

        var X2 = (double x) => x * x;
        Console.WriteLine("Интеграл от x² на [0, 3]:");
        double result2 = DefiniteIntegral.Solve(0, 3, X2, 1e-4, 4);
        Console.WriteLine($"Результат: {result2:F6} (ожидаемо: 9.0)\n");

        Console.WriteLine("Проверка многопоточности (медленная функция):");
        SlowFunction slowFunc = new SlowFunction();
        double result3 = DefiniteIntegral.Solve(0, 1, slowFunc.Compute, 1e-3, 4);
        Console.WriteLine($"Результат: {result3:F6}\n");
    }
}
public class SlowFunction
{
    public double Compute(double x)
    {
        Thread.Sleep(10);
        Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}: вычисляю f({x:F3})");
        return x * x;
    }
}
