using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace PF04
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
            DateTime now = DateTime.Now;

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

            #region 建立紀錄每個執行緒延遲多少時間
            List<double> delay = new List<double>();
            for (int i = 0; i < MAX; i++) { delay.Add(0); }
            #endregion

            #region 建立10000個執行緒
            for (int i = 0; i < MAX; i++)
            {
                int idx = i;
                Thread thread = new Thread(x =>
                {
                    delay[idx] = (DateTime.Now - now).TotalMilliseconds;
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
            now = DateTime.Now;
            for (int i = 0; i < MAX; i++)
            {
                threads[i].Start();
            }
            #endregion

            cde.Wait(); // 等待 10000 個執行緒全部執行完成
            stopwatch.Stop();
            stopMonitor = true;
            Console.WriteLine();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");

            Console.WriteLine($"Max {threadUsage.Max(x => x.NumberOfThreads)} Threads");

            foreach (var item in delay)
            {
                Console.Write($" {item} ");
            }
        }
    }
}
