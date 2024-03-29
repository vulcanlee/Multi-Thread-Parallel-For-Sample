# 由 Parallel.For 來看多執行緒程式設計

|類型|專案名稱|專案說明|
|-|-|-|
|範例體驗|PF01|同時執行 10000 次|
|範例體驗|PF02|同時執行 10000 次，觀察用了多少執行緒?|
|範例體驗|PF03|那就產生 10000 執行緒來同時執行|
|範例體驗|PF04|了解 10000 執行緒，執行緒開始執行會延遲多少時間|
|範例體驗|PF05|聽說 Task 很厲害，那就使用 10000 Tasks(Task.Run)|
|範例體驗|PF06|用Task.Factory.StartNew 建立 10000 Tasks|
|範例體驗|PF07|使用 Task.Delay 來取代封鎖 Block 的 Thread.Sleep|
|範例體驗|PF08|Parallel.For 改成使用 Task.Delay|
|範例體驗|PF09|修正 Parallel.For 改成使用 Task.Delay 造成的錯誤|
|範例體驗|PF10|Parallel.For 平行迴圈程式設計的同步特性|
|範例體驗|PF11|使用同步程式設計之迴圈處理特性|
|範例體驗|PF12|Parallel.For 平行迴圈程式設計的平行度預設特性|
|範例體驗|PF13|Parallel.For 平行迴圈程式設計的指定平行工作數目上限特性|
|範例體驗|PF14|Parallel.For 了解執行緒集區的運作方式|
|範例體驗|PF15|過多執行緒所造成記憶體不足的情況|
|範例體驗|PF16|透過執行緒集區(預設參數)取得過多執行緒的使用情況|
|範例體驗|PF17|透過執行緒集區(修正參數)取得過多執行緒的使用情況|
|範例體驗|PF18|Parallel.For 透過執行緒集區準備10000個執行緒的結果|
|挑戰練習|PF19|.NET Framework 使用 Parallel.For 非同步 呼叫 100 次 Web API|
|挑戰練習|PF20|.NET Framework 使用 100 Task 完成 非同步 呼叫 100 次 Web API|
|挑戰練習|PF21|.NET Core 使用 Parallel.For 非同步 呼叫 100 次 Web API|
|範例體驗|PF22|用 Parallel.For 設計計數器|
|範例體驗|PF23|觀察執行緒集區的執行緒配置數量|
||||

![由 Parallel.For 來看多執行緒程式設計](用最快的速度完成他，不考慮CPU記憶體.png)

