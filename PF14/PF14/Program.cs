using System;
using System.Threading;
using System.Threading.Tasks;

namespace PF14
{
    /// <summary>
    /// PF14|Parallel.For 了解執行緒集區的運作方式
    /// 
    /// 透過平行處理 200 次迴圈，了解執行緒集區的運作方式
    /// 採用方法為觀察每次 * 出現的數量與頻率，是否可以了解或者推敲其內部運作方式呢？
    /// 對於 Parallel.For 若無特別指定，預設將會從 執行緒集區 獲得執行緒
    /// 因此，了解執行緒集區運作方式，將會有助於更好的操控 Parallel.For API
    /// </summary>
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
