using System;
using System.Threading;

namespace PF15
{
    /// <summary>
    /// PF15|過多執行緒所造成記憶體不足的情況
    /// 
    /// 在這裡將會一次產生出 1 萬個執行緒，
    /// 由於每個執行緒預設會耗用 1MB 記憶體
    /// 因此，將會耗用 10 * 1000 * 1MB = 10 GB 的記憶體
    /// 
    /// 若在 32 位元模式下執行(切換方案平台為 x86)，其最多僅能夠支援 4GB 記憶體定址空間
    /// 所以，這樣就會造成記憶體不足 Out of Memory 的問題
    /// 然而，若在 64位元模式下，則這個範例不會造成記憶體不足的問題
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            int MAX = 10000;
            int SLEEP = 5 * 1000;

            #region 快速產生過多執行緒而造成的記憶體不足問題
            for (int i = 0; i < MAX; i++)
            {
                new Thread(() =>
                {
                    //Console.Write($"{i} ");
                    Thread.Sleep(SLEEP * 100);
                }).Start();
            }
            #endregion

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }
    }
}
