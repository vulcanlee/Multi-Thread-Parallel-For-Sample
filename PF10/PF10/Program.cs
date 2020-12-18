using System;
using System.Threading;
using System.Threading.Tasks;

namespace PF10
{
    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            Console.WriteLine($"Now:{DateTime.Now}");
            #region 當要平行處理 20 次迴圈，Parallel.For 內建同步機制，即迴圈內所有作業都完成，才算完成
            Parallel.For(0, 8, (i) =>
            {
                // 模擬隨機等待 1~2 秒鐘
                Thread.Sleep(random.Next(1000, 2000));
            });
            #endregion
            // 請觀察開始執行時間與結束時間輸出值
            Console.WriteLine($"Now:{DateTime.Now}");
        }
    }
}
