using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PF19
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int cost = 5000;
            int MaxTasks = 100;
            string APIEndPoint = $"https://businessblazor.azurewebsites.net/api/RemoteService/AddAsync/8/9/{cost}";
            //string APIEndPoint = $"https://businessblazor.azurewebsites.net/api/RemoteService/Add/8/9/{cost}";

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
