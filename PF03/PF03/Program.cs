using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace PF03
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
            List<Thread> threads = new List<Thread>();
            CountdownEvent cde = new CountdownEvent(MAX);

            #region 建立與統計最多執行緒數量的執行緒
            Thread monitorWorker = new Thread(() =>
            {
                while (!stopMonitor)
                {
                    Thread.Sleep(200);
                    Console.Write($"{Process.GetCurrentProcess().Threads.Count} ");
                    threadUsage.Add((DateTime.Now,
                        Process.GetCurrentProcess().Threads.Count));
                }
            });
            monitorWorker.Start();
            #endregion

            #region 建立10000個執行緒
            for (int i = 0; i < MAX; i++)
            {
                int idx = i;
                Thread thread = new Thread(x =>
                {
                    Thread.Sleep(SLEEP);
                    cde.Signal();
                });
                thread.IsBackground = true;
                threads.Add(thread);
            }
            #endregion

            #region 啟動並且執行10000個執行緒
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < MAX; i++)
            {
                threads[i].Start();
            }
            #endregion

            cde.Wait(); // 等待 10000 個執行緒全部執行完成
            stopMonitor = true;
            stopwatch.Stop();
            Thread.Sleep(500);
            Console.WriteLine();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");

            Console.WriteLine($"Max {threadUsage.Max(x => x.NumberOfThreads)} Threads");
        }
    }
}
