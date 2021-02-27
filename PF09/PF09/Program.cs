using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PF09
{
    class Program
    {
        static void Main(string[] args)
        {
            bool stopMonitor = false;

            #region 建立與統計最多執行緒數量的執行緒
            Thread monitorWorker = new Thread(() =>
            {
                while (!stopMonitor)
                {
                    Thread.Sleep(200);
                    Console.Write($"{Process.GetCurrentProcess().Threads.Count} ");
                }
            });
            monitorWorker.Start();
            #endregion

            CountdownEvent cde = new CountdownEvent(10000);
            var timer = new System.Timers.Timer();
            timer.Interval = 3500;
            timer.Elapsed += (s,e)=>
            {
                int foo = 999;
            };
            timer.Enabled = true;timer.Start();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Parallel.For(0, 10000, async (i) =>
            {
                //Thread.Sleep(5 * 1000);
                await Task.Delay(5 * 1000);
                cde.Signal();
            });

            cde.Wait();
            stopwatch.Stop();
            stopMonitor = true;
            Thread.Sleep(500);
            Console.WriteLine();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
