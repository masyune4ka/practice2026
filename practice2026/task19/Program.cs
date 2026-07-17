using ScottPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using task17;

namespace task19;

class Program
{
    public static int _activeCommands = 0;

    static void Main()
    {
        var exceptionHandler = new DefaultExceptionHandler();
        var serverThread = new ServerThread(exceptionHandler);

        for (int i = 1; i <= 5; i++)
        {
            Interlocked.Increment(ref _activeCommands);
            serverThread.AddCommand(new TestCommand(i, serverThread, 3));
        }
        serverThread.AddCommand(new CompletionCommand(serverThread));
        serverThread.Join(TimeSpan.FromSeconds(10));

        Console.WriteLine();
        Console.WriteLine("=== Запуск бенчмарка ===");

        int taskCount = 100;
        int[] threadCounts = { 1, 2, 4, 8 };
        List<double> times = new List<double>();

        foreach (int threads in threadCounts)
        {
            Console.WriteLine($"Тест {threads} поток(ов)...");
            double time = MeasureTime(threads, taskCount);
            times.Add(time);
            Console.WriteLine($"  Время: {time:F2} мс");
        }
        SaveReport(threadCounts, times, taskCount);
        GenerateGraph(threadCounts, times);

        Console.WriteLine("Готово!");
        Console.WriteLine("Файлы сохранены: report.txt, graph.png");
        Console.WriteLine("Нажмите любую клавишу...");
        Console.ReadKey();
    }
    static double MeasureTime(int threads, int taskCount)
    {
        var exceptionHandler = new DefaultExceptionHandler();
        var serverThread = new ServerThread(exceptionHandler);
        int completed = 0;

        for (int i = 0; i < taskCount; i++)
        {
            serverThread.AddCommand(new ActionCommand(() =>
            {
                Interlocked.Increment(ref completed);
                Thread.Sleep(1);
            }));
        }
        serverThread.AddCommand(new HardStopCommand(serverThread));

        Stopwatch sw = Stopwatch.StartNew();
        serverThread.Join(TimeSpan.FromSeconds(30));
        sw.Stop();

        return sw.Elapsed.TotalMilliseconds;
    }
    static void SaveReport(int[] threads, List<double> times, int taskCount)
    {
        using StreamWriter writer = new StreamWriter("report.txt");
        writer.WriteLine("=== ОТЧЕТ ПО ЗАДАНИЮ 19 ===");
        writer.WriteLine($"Количество задач: {taskCount}");
        writer.WriteLine("Время выполнения (мс):");

        for (int i = 0; i < threads.Length; i++)
        {
            writer.WriteLine($"{threads[i]} поток(ов): {times[i]:F2} мс");
        }
    }
    static void GenerateGraph(int[] threads, List<double> times)
    {
        Plot plot = new();
        plot.Add.Scatter(threads.ToArray(), times.ToArray());
        plot.Title("Время выполнения от количества потоков");
        plot.XLabel("Количество потоков");
        plot.YLabel("Время (мс)");
        plot.SavePng("graph.png", 800, 600);
    }
}
