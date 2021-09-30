using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace PF20
{
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

            #region 使用 for
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < MaxTasks; i++)
            {
                int idx = i;
                tasks.Add(Task.Run(async () =>
                {
                    HttpClient client = new HttpClient();
                    DateTime begin = DateTime.Now;
                    string result = await client.GetStringAsync(
                        APIEndPoint);
                    DateTime complete = DateTime.Now;
                    TimeSpan total = complete - begin;
                    Console.WriteLine($"{idx:D3} {begin:ss}-{complete:ss}={total.TotalSeconds:N3}    {result}");
                }));
            }
            await Task.WhenAll(tasks);
            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
            #endregion

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }
    }
}
