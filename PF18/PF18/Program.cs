﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PF18
{
    /// <summary>
    /// PF18|Parallel.For 透過執行緒集區準備10000個執行緒的結果
    /// 
    /// 在這個範例，將會使用 ThreadPool.SetMaxThreads 指定執行緒集區預先建議 1 萬個執行緒
    /// 接著使用 Parallel.For 來平行 1 萬次，每次休息 5 秒鐘
    /// 透過這個專案的執行結果來分析、理解為什麼會有這樣的執行表現
    /// 
    /// 雖然指定一開始取得 1 萬個執行緒，可是，在這個專案中，並不會一下就取得 1 萬個執行緒，
    /// 而是最多僅能夠同時使用到 8866 執行緒，因此，
    /// 想要一下子就有 1 萬個執行緒立馬來使用與執行，可以使用自行建立執行緒物件的方式
    /// </summary>
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
