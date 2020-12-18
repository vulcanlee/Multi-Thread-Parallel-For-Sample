using System;
using System.Threading;
using System.Threading.Tasks;

namespace PF13
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Now:{DateTime.Now}");
            #region 平行處理 20 次迴圈，平行處理的作業方式採用自行指定 ParallelOptions 物件值來運行

            ParallelOptions parallelOptions = new ParallelOptions()
            {
                // 指定平行度為 4，也就是一次最多僅有四個執行緒可以執行
                MaxDegreeOfParallelism = 4,
            };

            Parallel.For(0, 20, parallelOptions, (i) =>
             {
                 Console.Write($"{i} ");
                // 模擬隨機等待 1 秒鐘
                Thread.Sleep(3000);
             });
            #endregion
            // 請觀察開始執行時間與結束時間輸出值
            Console.WriteLine("");
            Console.WriteLine($"Now:{DateTime.Now}");

            // 底下是執行結果輸出內容
            //Now: 2020 / 12 / 18 下午 02:34:49
            //10 0 5 15 

            //Now: 2020 / 12 / 18 下午 02:34:49
            //10 0 5 15 1 11 6 16 

            //Now: 2020 / 12 / 18 下午 02:34:49
            //10 0 5 15 1 11 6 16 17 12 2 7 

            //Now: 2020 / 12 / 18 下午 02:34:49
            //10 0 5 15 1 11 6 16 17 12 2 7 13 8 18 3 

            //Now: 2020 / 12 / 18 下午 02:34:49
            //10 0 5 15 1 11 6 16 17 12 2 7 13 8 18 3 14 19 9 4
            //Now: 2020 / 12 / 18 下午 02:35:04   

        }
    }
}
