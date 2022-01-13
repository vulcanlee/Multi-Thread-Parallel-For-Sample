using System;
using System.Threading;
using System.Threading.Tasks;

namespace PF13
{
    /// <summary>
    /// PF13|Parallel.For 平行迴圈程式設計的指定平行工作數目上限特性
    /// 
    /// 使用這個範例來觀察與指定最大平行處理作業數量
    /// 透過建立 ParallelOptions.MaxDegreeOfParallelism 屬性，來修正平行處理作業的最大數量
    /// 預設若沒有特別指定， Parallel.For 會使用同時執行的作業數目沒有任何限制方式來運行
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Now:{DateTime.Now}");
            #region 平行處理 20 次迴圈，平行處理的作業方式採用自行指定 ParallelOptions 物件值來運行

            // MaxDegreeOfParallelism屬性會影響通過 Parallel 此實例之方法呼叫所執行的並行作業數目 ParallelOptions。
            // 正值的屬性值會將並行操作次數限制為設定的值。
            // 如果是 - 1，則同時執行的作業數目沒有任何限制。

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
