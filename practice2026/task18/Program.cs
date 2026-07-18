using ScottPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using task17;

namespace task18;
class Program
{
    static void Main()
    {
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
        var pool = new ThreadPool();
        pool.Start(threads);

        Stopwatch sw = Stopwatch.StartNew();

        for (int i = 0; i < taskCount; i++)
        {
            pool.EnqueueTask(new LongRunningTask(3));
        }
        pool.Stop();
        sw.Stop();

        return sw.Elapsed.TotalMilliseconds;
    }
    static void SaveReport(int[] threads, List<double> times, int taskCount)
    {
        using (StreamWriter writer = new StreamWriter("report.txt"))
        {
            writer.WriteLine("=== ОТЧЕТ ПО ЗАДАНИЮ 18 ===");
            writer.WriteLine($"Количество задач: {taskCount}");
            writer.WriteLine("Время выполнения (мс):");
            for (int i = 0; i < threads.Length; i++)
            {
                writer.WriteLine($"{threads[i]} поток(ов): {times[i]:F2} мс");
            }
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
public class LongRunningTask : ICommand
{
    private readonly int _totalSteps;
    private int _currentStep;
    public LongRunningTask(int totalSteps)
    {
        _totalSteps = totalSteps;
        _currentStep = 0;
    }
    public bool Execute()
    {
        int step = Interlocked.Increment(ref _currentStep);
        Thread.Sleep(10);
        return step >= _totalSteps;
    }
}
