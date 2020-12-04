using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PF08
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Parallel.For(0, 10000, async (i) =>
            {
                //Thread.Sleep(5 * 1000);
                await Task.Delay(5 * 1000);
            });
            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
