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
                 Console.Write($"({i}/T{Thread.CurrentThread.ManagedThreadId}) ");
                 // 模擬隨機等待 1 秒鐘
                 Thread.Sleep(3000);
             });
            #endregion
            // 請觀察開始執行時間與結束時間輸出值
            Console.WriteLine("");
            Console.WriteLine($"Now:{DateTime.Now}");

            // 底下是執行結果輸出內容
            //Now: 2020 / 12 / 18 下午 05:59:32
            //(10 / T5)(15 / T6)(0 / T1)(5 / T4) 

            //Now: 2020 / 12 / 18 下午 05:59:32
            //(10 / T5)(15 / T6)(0 / T1)(5 / T4) (1 / T1)(11 / T6)(6 / T8)(16 / T7) 

            //Now: 2020 / 12 / 18 下午 05:59:32
            //(10 / T5)(15 / T6)(0 / T1)(5 / T4) (1 / T1)(11 / T6)(6 / T8)(16 / T7) (17 / T7)(2 / T1)(7 / T8)(12 / T6)

            //Now: 2020 / 12 / 18 下午 05:59:32
            //(10 / T5)(15 / T6)(0 / T1)(5 / T4) (1 / T1)(11 / T6)(6 / T8)(16 / T7) (17 / T7)(2 / T1)(7 / T8)(12 / T6) (3 / T1)(8 / T9)(13 / T6)(18 / T8)

            //Now: 2020 / 12 / 18 下午 05:59:32
            //(10 / T5)(15 / T6)(0 / T1)(5 / T4) (1 / T1)(11 / T6)(6 / T8)(16 / T7) (17 / T7)(2 / T1)(7 / T8)(12 / T6) (3 / T1)(8 / T9)(13 / T6)(18 / T8) (19 / T8)(9 / T9)(4 / T1)(14 / T6)
            //Now: 2020 / 12 / 18 下午 05:59:47
        }
    }
}
