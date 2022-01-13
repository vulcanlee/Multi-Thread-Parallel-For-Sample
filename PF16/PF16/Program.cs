using System;
using System.Diagnostics;
using System.Threading;

namespace PF16
{
    /// <summary>
    /// PF16|透過執行緒集區(預設參數)取得過多執行緒的使用情況
    /// 
    /// 首先建立一個執行緒，該執行緒委派方法內將會是一個無窮迴圈，
    /// 每隔 0.5 秒來取得執行緒集區內擁有的執行緒數量
    /// 這裡是透過 ThreadPool.ThreadCount 來蒐集執行緒集區使用到的執行緒數量
    /// 
    /// 接著，將會執行一萬次迴圈，該迴圈內使用 ThreadPool.QueueUserWorkItem 來取得一個執行緒
    /// 在這裡的範例程式碼，將會透過 CountdownEvent 這個執行緒同步原始物件來做到執行緒同步的控制目的
    /// </summary>
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
