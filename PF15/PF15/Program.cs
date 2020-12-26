using System;
using System.Threading;

namespace PF15
{
    class Program
    {
        static void Main(string[] args)
        {
            int MAX = 10000;
            int SLEEP = 5 * 1000;

            #region 快速產生過多執行緒而造成的記憶體不足問題
            for (int i = 0; i < MAX; i++)
            {
                new Thread(() =>
                {
                    //Console.Write($"{i} ");
                    Thread.Sleep(SLEEP * 100);
                }).Start();
            }

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
            #endregion
        }
    }
}
