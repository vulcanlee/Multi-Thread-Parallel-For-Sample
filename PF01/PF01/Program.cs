using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PF01
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Parallel.For(0, 10000, _ =>
            {
                Thread.Sleep(5 * 1000);
            });
            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
