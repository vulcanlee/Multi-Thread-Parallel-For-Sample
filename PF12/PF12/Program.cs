using System;
using System.Threading;
using System.Threading.Tasks;

namespace PF12
{
    /// <summary>
    /// PF12|Parallel.For 平行迴圈程式設計的平行度預設特性
    /// 
    /// 透過這個範例程式來嘗試觀察 Parallel.For 的內部運作方式，嘗試推敲出如何最佳使用 Parallel.For 這個 API
    /// 觀察每個索引值出現位置，每次最多出現索引值的數量，每次執行結果是否都相同，這又代表甚麼意義呢？
    /// 這裡是設計作法：Parallel 會嘗試使用所有可用的處理器、無法取消，並以預設的 TaskScheduler () 為目標 TaskScheduler.Default
    /// 你可以明瞭這句話的意義嗎？
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Now:{DateTime.Now}");
            #region 平行處理 20 次迴圈，平行處理的作業方式採用預設設定值
            // 根據預設，類別上的方法 Parallel 會嘗試使用所有可用的處理器、無法取消，並以預設的 TaskScheduler () 為目標 TaskScheduler.Default
            // https://docs.microsoft.com/zh-tw/dotnet/api/system.threading.tasks.paralleloptions?view=net-5.0&WT.mc_id=DT-MVP-5002220
            Parallel.For(0, 20, (i) =>
            {
                Console.Write($"{i} ");
                // 模擬隨機等待 1 秒鐘
                Thread.Sleep(1000);
            });
            #endregion
            // 請觀察開始執行時間與結束時間輸出值
            Console.WriteLine("");
            Console.WriteLine($"Now:{DateTime.Now}");

            // 底下是執行結果輸出內容
            //Now: 2020 / 12 / 18 下午 02:22:08
            //14 10 4 6 2 8 0 12 16 1 3 11 9 13 7 5 17 15 18 19
            //Now: 2020 / 12 / 18 下午 02:22:12   
            //
            //Now: 2020 / 12 / 18 下午 02:22:44
            //6 2 10 8 12 0 16 4 14 18 1 3 17 5 15 13 9 7 11 19
            //Now: 2020 / 12 / 18 下午 02:22:47
        }
    }
}
