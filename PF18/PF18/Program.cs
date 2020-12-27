using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PF18
{
    class Program
    {
        static List<(DateTime current, int NumberOfThreads)> threadUsage =
            new List<(DateTime current, int NumberOfThreads)>();
        static void Main(string[] args)
        {
            int MAX = 10000;
            int SLEEP = 5 * 1000;
            bool stopMonitor = false;

            #region 調整 ThreadPool 的參數
            int avaWorkerThreads;
            int avaIocThreads;
            int maxWorkerThreads;
            int maxIocThreads;
            int minWorkerThreads;
            int minIocThreads;
            ThreadPool.GetMinThreads(out minWorkerThreads, out minIocThreads);
            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxIocThreads);
            Console.WriteLine($"現行執行環境的執行緒集區參數");
            Console.WriteLine($"可並行使用之執行緒集區的要求數:{maxWorkerThreads} / {maxIocThreads}");
            Console.WriteLine($"執行緒集區建立的執行緒最小數目:{minWorkerThreads} / {minIocThreads}");
            minWorkerThreads = MAX;
            minIocThreads = 100;
            maxWorkerThreads = MAX;
            ThreadPool.SetMaxThreads(maxWorkerThreads, maxIocThreads);
            ThreadPool.SetMinThreads(minWorkerThreads, minIocThreads);
            ThreadPool.GetMinThreads(out minWorkerThreads, out minIocThreads);
            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxIocThreads);
            Console.WriteLine();
            Console.WriteLine($"調整後的執行緒集區參數");
            Console.WriteLine($"可並行使用之執行緒集區的要求數:{maxWorkerThreads} / {maxIocThreads}");
            Console.WriteLine($"執行緒集區建立的執行緒最小數目:{minWorkerThreads} / {minIocThreads}");
            Console.WriteLine();
            #endregion

            #region 建立與統計最多執行緒數量的執行緒
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
            #endregion

            #region 蒐集執行緒集區使用到的執行緒數量
            bool isMonitor = true;
            new Thread(() =>
            {
                while (isMonitor)
                {
                    Console.Write($"{ThreadPool.ThreadCount} ");
                    Thread.Sleep(500);
                }
            }).Start();
            #endregion

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Parallel.For(0, MAX, _ =>
            {
                Thread.Sleep(SLEEP);
            });
            stopwatch.Stop();
            stopMonitor = true; isMonitor = false;
            Console.WriteLine();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");

            Console.WriteLine($"Max {threadUsage.Max(x => x.NumberOfThreads)} Threads");
        }
    }
}
