using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PF23
{
    // 觀察執行緒集區的執行緒配置數量
    class Program
    {
        static async Task Main(string[] args)
        {
            int MAX = 20;
            int SLEEP = 14 * 1000;
            bool stopMonitor = false;
            List<Task> tasks = new List<Task>();

            #region 建立與統計最多執行緒數量的執行緒
            Thread monitorWorker = new Thread(() =>
            {
                while (!stopMonitor)
                {
                    Thread.Sleep(200);
                    Console.Write($"{DateTime.Now.Second:D2}.{DateTime.Now.Millisecond:D3}-");
                    for (int i = 0; i < MAX; i++)
                    {
                        Console.Write($"{i:D2}:{(int)tasks[i].Status} ");
                    }
                    Console.WriteLine();
                }
            });
            #endregion

            Stopwatch stopwatch = new Stopwatch(); stopwatch.Start();

            for (int i = 0; i < MAX; i++)
            {
                tasks.Add(Task.Run(() => { Thread.Sleep(SLEEP); }));
            }

            monitorWorker.Start();

            await Task.WhenAll(tasks.ToArray());

            stopwatch.Stop(); stopMonitor = true;
            Console.WriteLine();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
