using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Frontend
{
    class Program
    {
        static void Main(string[] args)
        {
            int MAX = 100;
            string endPoint = "https://localhost:5001/Sendmail/";
            HttpClient client = new HttpClient();
            ConcurrentDictionary<int, string> callAPIResult = new ConcurrentDictionary<int, string>();
            List<int> cannotUseConcurrent = new List<int>();
            CountdownEvent cde = new CountdownEvent(MAX);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Parallel.For(0, MAX, async (i) =>
            {
                int idx = i;
                string result = await client
                .GetStringAsync($"{endPoint}{idx}/{idx}");
                callAPIResult.TryAdd(idx, result);
                cannotUseConcurrent.Add(idx);
                cde.Signal();
            });

            cde.Wait();
            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine();

            foreach ((int index, string result) in callAPIResult)
            {
                Console.WriteLine($"Index={index} > {result}");
            }
        }
    }
}
