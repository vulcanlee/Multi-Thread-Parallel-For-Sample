using System;
using System.Diagnostics;
using System.Threading;

namespace PF16
{
    class Program
    {
        static void Main(string[] args)
        {
            int MAX = 10000;
            int SLEEP = 5 * 1000;

            #region 蒐集執行緒集區使用到的執行緒數量
            bool isMonitor = true;
            new Thread(() =>
            {
                while (isMonitor)
                {
                    //Console.Write($"{Process.GetCurrentProcess().Threads.Count} ");
                    Console.Write($"{ThreadPool.ThreadCount} ");
                    Thread.Sleep(500);
                }
            }).Start();
            #endregion

            #region 透過執行緒集區取得過多執行緒的使用情況
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            CountdownEvent cde = new CountdownEvent(MAX);

            for (int i = 0; i < MAX; i++)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
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
