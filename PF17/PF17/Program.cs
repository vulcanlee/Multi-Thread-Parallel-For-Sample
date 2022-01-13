using System;
using System.Diagnostics;
using System.Threading;

namespace PF17
{
    /// <summary>
    /// PF17|透過執行緒集區(修正參數)取得過多執行緒的使用情況
    /// 
    /// 當處理程序啟動之後，執行緒集區預設將會建立該台主機擁有邏輯處理器數量的相對應執行緒數量
    /// 這樣的設計方式是要能夠在進行平行處理的時候，該處理程序內的執行緒儘可能都有邏輯處理器可以來使用
    /// 
    /// 在某些設計需求下，可能需要執行緒集區能夠預先建立較多數量的執行緒，以便當處理程序有需求的時候，
    /// 可以隨時透過執行緒集區來取得立即可用的執行緒來執行指定的委派方法程式碼
    /// 
    /// 這樣的修正要考量，是否應該要擴增該台主機邏輯處理器數量呢？
    /// 
    /// 要做到這樣的需求，需要呼叫 ThreadPool.SetMinThreads(minWorkerThreads, minIocThreads); 方法
    /// 指定預先建立的執行緒數量在 SetMinThreads 方法引數內，第一個表示工作執行緒數量，第二個表示 IO 執行緒數量
    /// 
    /// 最後透過 ThreadPool.QueueUserWorkItem 來取得執行緒，並且觀察螢幕輸出內容，請比較 PF16 專案執行結果
    /// </summary>
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
            minWorkerThreads = 500;
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
