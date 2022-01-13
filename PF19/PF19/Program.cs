using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PF19
{
    /// <summary>
    /// PF19|.NET Framework 使用 Parallel.For 非同步 呼叫 100 次 Web API
    /// 
    /// 在這個專案中，將會透過 HttpClient 物件，呼叫 100 次的 Web API
    /// 其中可以嘗試後端設計的同步 Web API 與 非同步 Web API
    /// 這個是 非同步 Web API : https://businessblazor.azurewebsites.net/api/RemoteService/AddAsync/8/9/{cost}
    /// 這個是   同步 Web API : https://businessblazor.azurewebsites.net/api/RemoteService/Add/8/9/{cost}
    /// 
    /// 請觀察與解釋在平行呼叫100次 Web API，採用 非同步 Web API 或者 同步 Web API 有何不同
    /// 為什麼採用 非同步 Web API 呼叫，卻可以有比較好的執行效能表現
    /// 可以透過執行後輸出結果內容，觀察每次呼叫 Web API 耗費的時間
    /// !!! 請注意，這台 雲端主機的 邏輯處理器僅有一個 !!!
    /// 
    /// 查看該 .NET 執行緒集區的參數，開啟該 URL :  https://businessblazor.azurewebsites.net/api/RemoteService/GetThreadPool
    /// 查詢執行緒集區的設定輸出內容說明
    /// W : Worker Thread(背景工作執行緒)
    /// C : I/O Completion Port Threads(非同步 I/O 執行緒)
    /// AW:8190 AC:1000 MaxW:8191 MaxC:1000 MinW:1 MinC:1 
    /// 
    /// 特別注意 : 因為在 Parallel.For 方法內指定了 非同步委派方法 (該方法有加入 async 修飾詞)，
    /// 因此，Parallel.For 的內建執行緒同步機制將會失效，
    /// 所以，在這裡要另外使用 CountdownEvent 這個執行緒同步原始物件來做到等到所有的工作都完成的目的
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            int cost = 5000;
            int MaxTasks = 100;
            string APIEndPoint = $"https://businessblazor.azurewebsites.net/api/RemoteService/AddAsync/8/9/{cost}";
            //string APIEndPoint = $"https://businessblazor.azurewebsites.net/api/RemoteService/Add/8/9/{cost}";

            #region 宣告定時器，用來觀察用到多少工作與執行緒
            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += (s, e) =>
            {
                // 在這裡設定中斷點，觀察用到多少工作與執行緒
                int foo = 0;
            };
            timer.Start();
            #endregion

            Stopwatch stopwatch = new Stopwatch(); stopwatch.Start();

            #region 使用 Parallel.For
            CountdownEvent cde = new CountdownEvent(MaxTasks);
            Parallel.For(0, MaxTasks, async (i) =>
            {
                int idx = i;
                HttpClient client = new HttpClient();
                DateTime begin = DateTime.Now;
                string result = await client.GetStringAsync(APIEndPoint);
                DateTime complete = DateTime.Now;
                TimeSpan total = complete - begin;
                Console.WriteLine($"{idx:D3} {begin:ss}-{complete:ss}={total.TotalSeconds:N3}    {result}");
                cde.Signal();
            });

            cde.Wait();
            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
            #endregion

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }
    }
}
