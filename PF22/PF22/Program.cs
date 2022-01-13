using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PF22
{
    /// <summary>
    /// PF22|用 Parallel.For 設計計數器
    /// 
    /// 在這個專案下，將會執行 1 萬次迴圈，並且有個計數器變數，每次迴圈執行一次，該變數就會加 1
    /// 透過使用同步程式設計 for 這個表示式 來進行同步計算，觀察需要花費多久時間
    /// 另外，
    /// 透過 Parallel.For 這個 API ，來進行平行計算，觀察需要花費多久時間
    /// 
    /// 透過這裡個範例來理解到 平行計算的威力與如何簡單設計出這樣的程式碼
    /// 
    /// 在 for 下
    /// 由於這樣的範例程式碼，在同步執行下，竟然比起平行計算快得多
    /// 因此，想要設計成為平行計算的程式碼，事前還是建議先量測執行效能
    /// 再改成平行計算程式碼之後，要再度比較執行效能與結果值，
    /// 確認是有效的平行計算程式碼
    /// 
    /// 在 Parallel.For 下
    /// 若沒有使用 lock 關鍵字，多執行幾次，就會觀察到雖然執行速度很快，
    /// 不過，有些時候加總數值是不正確的(應該要為 10000)
    /// 若有使用 lock 關鍵字，執行速度雖然有點慢，
    /// 不過，有些時候加總數值卻都是不正確的(皆為 10000)
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int counter = 0;
            int MAX = 10000;

            for (int i = 0; i < MAX; i++)
            {
                counter = counter + 1;
            }


            //object locker = new object();
            //Parallel.For(0, MAX, i =>
            //{
            //    // 嘗試註解這行敘述，多執行幾次，執行結果會有出入，
            //    // 這樣的表現稱之為 沒有 執行緒安全 特性
            //    //
            //    // 若加入 lock 這個敘述，則具有執行緒安全特性，但會執行效能會有影響
            //    lock (locker)
            //    {
            //        counter = counter + 1;
            //    }
            //});

            stopwatch.Stop();
            Console.WriteLine($"Counter={counter}, {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
