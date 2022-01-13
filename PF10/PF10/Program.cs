using System;
using System.Threading;
using System.Threading.Tasks;

namespace PF10
{
    /// <summary>
    /// PF10|Parallel.For 平行迴圈程式設計的同步特性
    /// 
    /// 在這個範例中，說明了 Parallel.For Parallel.ForEach 內建了 執行緒同步 處理物件，
    /// 需要等到所有平行處理委派方法都處理完成後，才會結束，否則，將會採用 封鎖 Block 方式來等待
    /// 這裡平行迴圈共有 8 個，模擬每個處理作業需要花費不同的時間才能完成
    /// 從結果可以觀察到，要等到最多
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            Console.WriteLine($"Now:{DateTime.Now}");
     
            #region 當要平行處理 20 次迴圈，Parallel.For 內建同步機制，即迴圈內所有作業都完成，才算完成
            Parallel.For(0, 8, (i) =>
            {
                // 模擬隨機等待 1~5 秒鐘
                Thread.Sleep(random.Next(1000, 5000));
            });
            #endregion
      
            // 請觀察開始執行時間與結束時間輸出值
            Console.WriteLine($"Now:{DateTime.Now}");

            // 底下是執行結果輸出內容
            // Now: 2020 / 12 / 18 下午 12:47:43
            // Now: 2020 / 12 / 18 下午 12:47:45
        }
    }
}
