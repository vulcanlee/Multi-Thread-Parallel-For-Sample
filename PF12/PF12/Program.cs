using System;
using System.Threading;
using System.Threading.Tasks;

namespace PF12
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Now:{DateTime.Now}");
            #region 當要平行處理 20 次迴圈，Parallel.For 內建同步機制，即迴圈內所有作業都完成，才算完成
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
