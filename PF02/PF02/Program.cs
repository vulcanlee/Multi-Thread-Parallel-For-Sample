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
            int MAX = 10000;
            int SLEEP = 5 * 1000;
            bool stopMonitor = false;

            #region 在此設定中斷點，用來觀察執行緒與與工作的數量有多少

            var timer = new System.Timers.Timer();
            // 調整要觀察的時間點
            timer.Interval = 17000;
            timer.Elapsed += (s, e) =>
            {
                // 請宣告底下這行為除錯中斷點
                int foo = 999;
            };
            timer.Enabled = true; timer.Start();
            #endregion

            #region 建立與統計最多執行緒數量的執行緒
            Thread monitorWorker = new Thread(() =>
            {
                while (!stopMonitor)
                {
                    //Console.Write($"{Process.GetCurrentProcess().Threads.Count} ");
                    Thread.Sleep(200);
                    threadUsage.Add((DateTime.Now,
                        Process.GetCurrentProcess().Threads.Count));
                }
            });
            monitorWorker.Start();
            #endregion

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ParallelOptions options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = -1,
                TaskScheduler = TaskScheduler.Default
            };
            Parallel.For(0, MAX, _ =>
            {
                Thread.Sleep(SLEEP);
            });
            stopwatch.Stop();
            stopMonitor = true;
            Console.WriteLine();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");

            Console.WriteLine($"Max {threadUsage.Max(x => x.NumberOfThreads)} Threads");
        }
    }
}
