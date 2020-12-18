using System;
using System.Threading;
using System.Threading.Tasks;

namespace PF14
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Now:{DateTime.Now}");
            #region 平行處理 200 次迴圈，了解執行緒集區的運作方式
            Parallel.For(0, 200, (i) =>
            {
                Console.Write($"*");
                // 模擬等待 5 秒鐘
                Thread.Sleep(5000);
            });
            #endregion
            // 請觀察開始執行時間與結束時間輸出值
            Console.WriteLine("");
            Console.WriteLine($"Now:{DateTime.Now}");
        }
    }
}
