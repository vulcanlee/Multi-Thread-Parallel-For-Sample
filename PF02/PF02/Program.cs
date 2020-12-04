using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace PF02
{
    class Program
    {
        static List<(DateTime current, int NumberOfThreads)> threadUsage =
            new List<(DateTime current, int NumberOfThreads)>();
        static void Main(string[] args)
        {
            int MAX = 200;
            int SLEEP = 5 * 1000;
            bool stopMonitor = false;
            Thread monitorWorker = new Thread(() =>
             {
                 while (!stopMonitor)
                 {
                     Thread.Sleep(200);
                     threadUsage.Add((DateTime.Now,
                         Process.GetCurrentProcess().Threads.Count));
                 }
             });
            monitorWorker.Start();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Parallel.For(0, MAX, _ =>
            {
                Thread.Sleep(SLEEP);
            });
            stopwatch.Stop();
            stopMonitor = true;
            Console.WriteLine();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");

            Console.WriteLine($"Max {threadUsage.Max(x=>x.NumberOfThreads)} Threads");
        }
    }
}
