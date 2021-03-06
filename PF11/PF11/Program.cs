﻿using System;
using System.Threading;

namespace PF11
{
    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            Console.WriteLine($"Now:{DateTime.Now}");
            #region 當要同步程式設計，要執行 20 次迴圈，所完成時間為迴圈內所有工作處理時間總和
            for (int i = 0; i < 8; i++)
            {
                // 模擬隨機等待 1~2 秒鐘
                Thread.Sleep(random.Next(1000, 2000));
            }
            #endregion
            // 請觀察開始執行時間與結束時間輸出值
            Console.WriteLine($"Now:{DateTime.Now}");

            // 底下是執行結果輸出內容
            // Now:2020 / 12 / 18 下午 12:45:56
            // Now: 2020 / 12 / 18 下午 12:46:09
        }
    }
}
