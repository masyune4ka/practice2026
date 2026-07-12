using System;
using System.Threading;

namespace task14;

public static class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        double result = 0.0;
        double segmentLength = (b - a) / threadsNumber;
        Thread[] threads = new Thread[threadsNumber];

        for (int i = 0; i < threadsNumber; i++)
        {
            double segmentStart = a + i * segmentLength;
            double segmentEnd = segmentStart + segmentLength;

            threads[i] = new Thread(() =>
            {
                double localResult = 0.0;

                int stepsCount = (int)((segmentEnd - segmentStart) / step);
                for (int j = 0; j < stepsCount; j++)
                {
                    double x = segmentStart + j * step;
                    double nextX = x + step;
                    double trapezoid = (function(x) + function(nextX)) / 2.0 * step;
                    localResult += trapezoid;
                }
                double newResult;
                double currentResult;
                do
                {
                    currentResult = result;
                    newResult = currentResult + localResult;
                } while (Interlocked.CompareExchange(ref result, newResult, currentResult) != currentResult);
            });
            threads[i].Start();
        }
        for (int i = 0; i < threadsNumber; i++)
        {
            threads[i].Join();
        }
        return result;
    }
}
