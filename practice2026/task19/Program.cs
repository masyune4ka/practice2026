using ScottPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using task17;

namespace task19;

class Program
{
    public static int _activeCommands = 0;

    static void Main()
    {
        Console.WriteLine("=== Демонстрация: 5 команд по 3 вызова ===");
        var exceptionHandler = new DefaultExceptionHandler();
        var serverThread = new ServerThread(exceptionHandler);
        for (int i = 1; i <= 5; i++)
        {
            Interlocked.Increment(ref _activeCommands);
            serverThread.AddCommand(new TestCommand(i, serverThread, 3));
        }
        serverThread.AddCommand(new CompletionCommand(serverThread));

        Console.WriteLine("Запуск потока...");
        serverThread.Join(TimeSpan.FromSeconds(10));
        Console.WriteLine("Поток успешно остановлен (HardStop).\n");

        Console.WriteLine("=== Сбор данных для отчета ===");
        int[] maxCallsSteps = { 1, 2, 3, 4, 5 };
        List<double> times = new List<double>();

        foreach (int steps in maxCallsSteps)
        {
            Console.WriteLine($"Тест: команды требуют {steps} вызовов...");
            double time = MeasureLongRunningTask(steps);
            times.Add(time);
            Console.WriteLine($"  Время: {time:F2} мс");
        }
        SaveReport(maxCallsSteps, times);
        GenerateGraph(maxCallsSteps, times);

        Console.WriteLine("\nГотово! Файлы сохранены: report.txt, graph.png");
        Console.WriteLine("Нажмите любую клавишу...");
        Console.ReadKey();
    }
    static double MeasureLongRunningTask(int maxCalls)
    {
        _activeCommands = 0;

        var exceptionHandler = new DefaultExceptionHandler();
        var serverThread = new ServerThread(exceptionHandler);

        int commandsCount = 10;
        for (int i = 1; i <= commandsCount; i++)
        {
            Interlocked.Increment(ref _activeCommands);
            serverThread.AddCommand(new TestCommand(i, serverThread, maxCalls));
        }
        serverThread.AddCommand(new CompletionCommand(serverThread));

        Stopwatch sw = Stopwatch.StartNew();
        serverThread.Join(TimeSpan.FromSeconds(30));
        sw.Stop();

        return sw.Elapsed.TotalMilliseconds;
    }
    static void SaveReport(int[] steps, List<double> times)
    {
        using StreamWriter writer = new StreamWriter("report.txt");
        writer.WriteLine("=== ОТЧЕТ ПО ЗАДАНИЮ 19 ===");
        writer.WriteLine("Время выполнения 10 команд в зависимости от их 'длительности' (количества вызовов Execute):");
        for (int i = 0; i < steps.Length; i++)
        {
            writer.WriteLine($"{steps[i]} шагов (вызовов): {times[i]:F2} мс");
        }
    }
    static void GenerateGraph(int[] steps, List<double> times)
    {
        Plot plot = new();
        plot.Add.Scatter(steps.ToArray(), times.ToArray());
        plot.Title("Время выполнения длительных операций");
        plot.XLabel("Количество шагов (вызовов Execute на команду)");
        plot.YLabel("Общее время (мс)");
        plot.SavePng("graph.png", 800, 600);
    }
}
