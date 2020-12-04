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
            CountdownEvent cde = new CountdownEvent(10000);
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
            Console.WriteLine();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
