using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PF22
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int counter = 0;
            int MAX = 10000;

            //for (int i = 0; i < MAX; i++)
            //{
            //    counter = counter + 1;
            //}


            object locker = new object();
            Parallel.For(0, MAX, i =>
            {
                lock (locker)
                {
                    counter = counter + 1;
                }
            });

            stopwatch.Stop();
            Console.WriteLine($"Counter={counter}, {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
