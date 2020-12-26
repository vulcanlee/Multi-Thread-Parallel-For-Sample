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
            int MAX = 10000;
            int SLEEP = 5 * 1000;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            #region 開始平行執行10000個作業
            Parallel.For(0, MAX, _ =>
            {
                Thread.Sleep(SLEEP);
            });
            #endregion

            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
