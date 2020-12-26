using System;
using System.Diagnostics;
using System.Threading;

namespace PF17
{
    class Program
    {
        static void Main(string[] args)
        {
            int MAX = 10000;
            int SLEEP = 5 * 1000;

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
            minWorkerThreads = 3000;
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

            #region 蒐集該執行程序使用到的執行緒數量
            bool isMonitor = true;
            new Thread(() =>
            {
                while (isMonitor)
                {
                    ThreadPool.GetAvailableThreads(out avaWorkerThreads, out avaIocThreads);
                    ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxIocThreads);
                    //Console.Write($"{maxWorkerThreads - avaWorkerThreads}-{Process.GetCurrentProcess().Threads.Count} ");
                    Console.Write($"{Process.GetCurrentProcess().Threads.Count} ");
                    Thread.Sleep(200);
                }
            }).Start();
            #endregion

            #region 快速產生過多執行緒而造成的記憶體不足問題
            //for (int i = 0; i < MAX; i++)
            //{
            //    new Thread(() =>
            //    {
            //        //Console.Write($"{i} ");
            //        Thread.Sleep(SLEEP * 1000);
            //    }).Start();
            //}

            //Console.WriteLine("Press any key for continuing...");
            //Console.ReadKey();
            #endregion

            #region 透過執行緒集區取得過多執行緒的使用情況
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            CountdownEvent cde = new CountdownEvent(MAX);

            for (int i = 0; i < MAX; i++)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    //Console.Write($"{i} ");
                    Thread.Sleep(SLEEP);
                    cde.Signal();
                });
            }

            cde.Wait();
            isMonitor = false;
            stopwatch.Stop();
            #endregion
         
            Console.WriteLine("Runing 10000 times by threadpool...");
            Console.WriteLine();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
