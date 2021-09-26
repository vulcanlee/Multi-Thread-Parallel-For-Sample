# 使用Parallel.For方法，呼叫 Web API 做兩數相加

這篇文章將會包含底下的內容

* [建立測試用的 Web API](#建立測試用的-web-api)
* [建立一個 主控台應用程式 (.NET Framework)](#建立一個-主控台應用程式-net-framework)
* [建立一個 主控台應用程式 (.NET Core)](#建立一個-主控台應用程式-net-core)
* [進行測試的步驟](#進行測試的步驟)

## 建立測試用的 Web API

建立一個 ASP.NET Core 的 Web API 專案，新增一個 控制器，其程式碼如下

這個 API 提供了這些功能

* 使用非同步 API，進行兩個整數的相加
* 使用同步 API，進行兩個整數的相加
* 可以查詢執行緒集區的運行參數
* 設定執行緒集區隨著提出新要求，視需要建立的執行緒最小數目
* 設定可並行使用之執行緒集區的要求數目。 超過該數目的所有要求會繼續佇列，直到可以使用執行緒集區執行緒為止

```csharp
[Route("api/[controller]")]
[ApiController]
public class RemoteServiceController : ControllerBase
{
    [HttpGet("AddAsync/{value1}/{value2}/{delay}")]
    public async Task<string> AddAsync(int value1, int value2, int delay)
    {
        DateTime Begin = DateTime.Now;
 
        #region 取得當前執行緒集區的狀態
        int workerThreadsAvailable;
        int completionPortThreadsAvailable;
        ThreadPool.GetAvailableThreads(out workerThreadsAvailable, out completionPortThreadsAvailable);
        #endregion
 
        await Task.Delay(delay);
 
        int sum = value1 + value2;
 
        DateTime Complete = DateTime.Now;
        return $"Result:{sum} " +
            $"AW:{workerThreadsAvailable} AC:{completionPortThreadsAvailable}" +
         $" ({Begin:ss} - {Complete:ss} = {(Complete - Begin).TotalSeconds})";
    }
    [HttpGet("Add/{value1}/{value2}/{delay}")]
    public string Add(int value1, int value2, int delay)
    {
        DateTime Begin = DateTime.Now;
        #region 取得當前執行緒集區的狀態
        int workerThreadsAvailable;
        int completionPortThreadsAvailable;
        ThreadPool.GetAvailableThreads(out workerThreadsAvailable, out completionPortThreadsAvailable);
        #endregion
 
        Thread.Sleep(delay);
 
        int sum = value1 + value2;
 
        DateTime Complete = DateTime.Now;
        return $"Result:{sum} " +
         $"AW:{workerThreadsAvailable} AC:{completionPortThreadsAvailable}" +
         $" ({Begin:ss} - {Complete:ss} = {(Complete-Begin).TotalSeconds})";
    }
    [HttpGet("SetThreadPool/{value1}/{value2}")]
    public string SetThreadPool(int value1, int value2)
    {
        ThreadPool.SetMinThreads(value1, value2);
 
        string result = "OK";
        int workerThreadsAvailable;
        int completionPortThreadsAvailable;
        int workerThreadsMax;
        int completionPortThreadsMax;
        int workerThreadsMin;
        int completionPortThreadsMin;
        ThreadPool.GetAvailableThreads(out workerThreadsAvailable, out completionPortThreadsAvailable);
        ThreadPool.GetMaxThreads(out workerThreadsMax, out completionPortThreadsMax);
        ThreadPool.GetMinThreads(out workerThreadsMin, out completionPortThreadsMin);
 
        DateTime Complete = DateTime.Now;
        result = "OK " + $"AW:{workerThreadsAvailable} AC:{completionPortThreadsAvailable}" +
            $" MaxW:{workerThreadsMax} MaxC:{completionPortThreadsMax}" +
            $" MinW:{workerThreadsMin} MinC:{completionPortThreadsMin} ";
 
        return result;
    }
    [HttpGet("SetMaxThreadPool/{value1}/{value2}")]
    public string SetMaxThreadPool(int value1, int value2)
    {
        ThreadPool.SetMaxThreads(value1, value2);
 
        string result = "OK";
        int workerThreadsAvailable;
        int completionPortThreadsAvailable;
        int workerThreadsMax;
        int completionPortThreadsMax;
        int workerThreadsMin;
        int completionPortThreadsMin;
        ThreadPool.GetAvailableThreads(out workerThreadsAvailable, out completionPortThreadsAvailable);
        ThreadPool.GetMaxThreads(out workerThreadsMax, out completionPortThreadsMax);
        ThreadPool.GetMinThreads(out workerThreadsMin, out completionPortThreadsMin);
 
        DateTime Complete = DateTime.Now;
        result = "OK " + $"AW:{workerThreadsAvailable} AC:{completionPortThreadsAvailable}" +
            $" MaxW:{workerThreadsMax} MaxC:{completionPortThreadsMax}" +
            $" MinW:{workerThreadsMin} MinC:{completionPortThreadsMin} ";
 
        return result;
    }
    [HttpGet("GetThreadPool")]
    public string GetThreadPool()
    {
        string result = "";
        int workerThreadsAvailable;
        int completionPortThreadsAvailable;
        int workerThreadsMax;
        int completionPortThreadsMax;
        int workerThreadsMin;
        int completionPortThreadsMin;
        ThreadPool.GetAvailableThreads(out workerThreadsAvailable, out completionPortThreadsAvailable);
        ThreadPool.GetMaxThreads(out workerThreadsMax, out completionPortThreadsMax);
        ThreadPool.GetMinThreads(out workerThreadsMin, out completionPortThreadsMin);
 
        DateTime Complete = DateTime.Now;
        result = " " + $"AW:{workerThreadsAvailable} AC:{completionPortThreadsAvailable}" +
            $" MaxW:{workerThreadsMax} MaxC:{completionPortThreadsMax}" +
            $" MinW:{workerThreadsMin} MinC:{completionPortThreadsMin} ";
 
        return result;
    }
}
```

## 建立一個 主控台應用程式 NET Framework

使用底下程式碼來替換 Program.cs 檔案內容

```csharp
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PF19
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int cost = 5000;
            int MaxTasks = 10;
            string APIEndPoint = $"https://businessblazor.azurewebsites.net/api/RemoteService/AddAsync/8/9/{cost}";
            //string APIEndPoint = $"https://businessblazor.azurewebsites.net/api/RemoteService/Add/8/9/{cost}";

            Stopwatch stopwatch = new Stopwatch(); stopwatch.Start();

            #region 使用 Parallel.For
            CountdownEvent cde = new CountdownEvent(MaxTasks);
            Parallel.For(0, MaxTasks, async (i) =>
            {
                int idx = i;
                HttpClient client = new HttpClient();
                DateTime begin = DateTime.Now;
                string result = await client.GetStringAsync(APIEndPoint);
                DateTime complete = DateTime.Now;
                TimeSpan total = complete - begin;
                Console.WriteLine($"{idx:D3} {begin:ss}-{complete:ss}={total.TotalSeconds:N3}    {result}");
                cde.Signal();
            });

            cde.Wait();
            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
            #endregion

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }
    }
}
```


## 建立一個 主控台應用程式 NET Core

使用底下程式碼來替換 Program.cs 檔案內容

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace PF20
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int cost = 5000;
            int MaxTasks = 100;
            string APIEndPoint = $"https://businessblazor.azurewebsites.net/api/RemoteService/AddAsync/8/9/{cost}";
            //string APIEndPoint = $"https://businessblazor.azurewebsites.net/api/RemoteService/Add/8/9/{cost}";

            Stopwatch stopwatch = new Stopwatch(); stopwatch.Start();

            #region 使用 for
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < MaxTasks; i++)
            {
                int idx = i;
                tasks.Add(Task.Run(async () =>
                {
                    HttpClient client = new HttpClient();
                    DateTime begin = DateTime.Now;
                    string result = await client.GetStringAsync(
                        APIEndPoint);
                    DateTime complete = DateTime.Now;
                    TimeSpan total = complete - begin;
                    Console.WriteLine($"{idx:D3} {begin:ss}-{complete:ss}={total.TotalSeconds:N3}    {result}");
                }));
            }
            await Task.WhenAll(tasks);
            stopwatch.Stop();
            Console.WriteLine();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
            #endregion

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }
    }
}
```

## 進行測試的步驟

* 比較 .NET Framework 與 .NET Core 兩個框架下來執行 Parallel.For 平行呼叫遠端非同步，會得到什麼的差異呢？
* 在 .NET Framework 專案下，執行 Parallel.For 平行呼叫遠端同步
* 改善在 .NET Framework 專案下，執行 Parallel.For 平行呼叫遠端同步的執行產出量
* 在 .NET Framework 專案下，模擬執行集區可用數量不夠的時候，會有什麼問題？

### .NET Framework - Parallel.For 平行呼叫遠端非同步

在 .NET Framework 專案內，修正 APIEndPoint 使用 `$"https://businessblazor.azurewebsites.net/api/RemoteService/AddAsync/8/9/{cost}";` 字串參數，這裡將會使用 Parallel.For 平行呼叫遠端的非同步 API

使用底下的程式碼，平行呼叫遠端非同步 Web API 100 次，每次呼叫 Web API 將會 5 秒鐘後才會得到執行結果

```csharp
int cost = 5000;
int MaxTasks = 100;
```

按下 [Ctrl] + [F5] ，使用 [啟動但不偵錯] 方式來執行這個專案

這裡是執行結果

```
088 06-12=6.058    Result:17 AW:680 AC:1000 (06 - 11 = 5.0053292)
021 06-12=6.058    Result:17 AW:680 AC:1000 (06 - 11 = 5.0054157)
058 06-12=6.065    Result:17 AW:680 AC:1000 (06 - 11 = 4.9952574)
063 06-12=6.063    Result:17 AW:680 AC:1000 (06 - 11 = 5.0017723)
010 06-12=6.065    Result:17 AW:680 AC:1000 (06 - 11 = 4.9948665)
054 06-12=6.058    Result:17 AW:680 AC:1000 (06 - 11 = 5.002422)
001 06-12=6.061    Result:17 AW:680 AC:1000 (06 - 11 = 4.9993929)
002 06-12=6.058    Result:17 AW:680 AC:1000 (06 - 11 = 4.9976607)
018 06-12=6.058    Result:17 AW:680 AC:1000 (06 - 11 = 4.9999552)
060 06-12=6.085    Result:17 AW:680 AC:1000 (06 - 11 = 4.9882839)
056 06-12=6.058    Result:17 AW:680 AC:1000 (06 - 11 = 5.0063663)
017 06-12=6.065    Result:17 AW:680 AC:1000 (06 - 11 = 4.9951433)
051 06-12=6.067    Result:17 AW:680 AC:1000 (06 - 11 = 5.008133)
083 06-12=6.067    Result:17 AW:680 AC:1000 (06 - 11 = 4.9889155)
020 06-12=6.065    Result:17 AW:680 AC:1000 (06 - 11 = 4.9977985)
026 06-12=6.071    Result:17 AW:680 AC:1000 (06 - 11 = 4.9885035)
053 06-12=6.075    Result:17 AW:680 AC:1000 (06 - 11 = 5.0075661)
081 06-12=6.076    Result:17 AW:680 AC:1000 (06 - 11 = 4.9890553)
089 06-12=6.074    Result:17 AW:680 AC:1000 (06 - 11 = 4.9883827)
033 06-12=6.073    Result:17 AW:680 AC:1000 (06 - 11 = 4.9949769)
095 06-12=6.058    Result:17 AW:680 AC:1000 (06 - 11 = 5.0129191)
034 06-12=6.067    Result:17 AW:680 AC:1000 (06 - 11 = 4.9886922)
065 06-12=6.093    Result:17 AW:680 AC:1000 (06 - 11 = 5.0012552)
064 06-12=6.071    Result:17 AW:680 AC:1000 (06 - 11 = 5.0062801)
093 06-12=6.092    Result:17 AW:680 AC:1000 (06 - 11 = 5.0016541)
048 06-12=6.112    Result:17 AW:680 AC:1000 (06 - 11 = 4.9890471)
070 06-12=6.094    Result:17 AW:680 AC:1000 (06 - 11 = 4.9884345)
067 06-12=6.092    Result:17 AW:680 AC:1000 (06 - 11 = 5.0014999)
084 06-12=6.110    Result:17 AW:680 AC:1000 (06 - 11 = 4.9888365)
080 06-12=6.067    Result:17 AW:680 AC:1000 (06 - 11 = 4.9888133)
059 06-12=6.095    Result:17 AW:680 AC:1000 (06 - 11 = 4.9885087)
076 06-12=6.094    Result:17 AW:680 AC:1000 (06 - 11 = 5.0019227)
055 06-12=6.071    Result:17 AW:680 AC:1000 (06 - 11 = 4.9947294)
066 06-12=6.094    Result:17 AW:680 AC:1000 (06 - 11 = 4.9886745)
077 06-12=6.095    Result:17 AW:680 AC:1000 (06 - 11 = 4.9878139)
052 06-12=6.097    Result:17 AW:680 AC:1000 (06 - 11 = 4.9880875)
079 06-12=6.097    Result:17 AW:680 AC:1000 (06 - 11 = 4.9879981)
003 06-12=6.095    Result:17 AW:680 AC:1000 (06 - 11 = 4.9882753)
082 06-12=6.099    Result:17 AW:680 AC:1000 (06 - 11 = 5.001824)
072 06-12=6.111    Result:17 AW:680 AC:1000 (06 - 11 = 5.0013517)
043 06-12=6.092    Result:17 AW:680 AC:1000 (06 - 11 = 5.0020695)
085 06-12=6.094    Result:17 AW:680 AC:1000 (06 - 11 = 4.9881786)
086 06-12=6.100    Result:17 AW:680 AC:1000 (06 - 11 = 4.9885912)
005 06-12=6.098    Result:17 AW:680 AC:1000 (06 - 11 = 4.9879047)
013 06-12=6.107    Result:17 AW:680 AC:1000 (06 - 11 = 5.0173099)
030 06-12=6.107    Result:17 AW:680 AC:1000 (06 - 11 = 5.0054771)
035 06-12=6.107    Result:17 AW:680 AC:1000 (06 - 11 = 5.0171477)
024 06-12=6.126    Result:17 AW:680 AC:1000 (06 - 11 = 5.0056002)
009 06-12=6.108    Result:17 AW:680 AC:1000 (06 - 11 = 5.0169143)
061 06-12=6.107    Result:17 AW:680 AC:1000 (06 - 11 = 5.0051077)
050 06-12=6.110    Result:17 AW:680 AC:1000 (06 - 11 = 5.0063425)
011 06-12=6.113    Result:17 AW:680 AC:1000 (06 - 11 = 5.0168076)
098 06-12=6.117    Result:17 AW:680 AC:1000 (06 - 11 = 5.0062021)
078 06-12=6.118    Result:17 AW:680 AC:1000 (06 - 11 = 5.0176911)
029 06-12=6.119    Result:17 AW:680 AC:1000 (06 - 11 = 5.0057297)
068 06-12=6.119    Result:17 AW:680 AC:1000 (06 - 11 = 5.0170451)
027 06-12=6.129    Result:17 AW:680 AC:1000 (06 - 11 = 5.0167061)
008 06-12=6.128    Result:17 AW:680 AC:1000 (06 - 11 = 5.0052282)
007 06-12=6.128    Result:17 AW:680 AC:1000 (06 - 11 = 5.0058545)
025 06-12=6.129    Result:17 AW:680 AC:1000 (06 - 11 = 5.0059616)
014 06-12=6.130    Result:17 AW:680 AC:1000 (06 - 11 = 5.0048759)
092 06-12=6.130    Result:17 AW:680 AC:1000 (06 - 11 = 5.0037516)
016 06-12=6.131    Result:17 AW:680 AC:1000 (06 - 11 = 5.0060675)
039 06-12=6.131    Result:17 AW:680 AC:1000 (06 - 11 = 5.0042722)
049 06-12=6.134    Result:17 AW:680 AC:1000 (06 - 11 = 5.0047712)
062 06-12=6.136    Result:17 AW:680 AC:1000 (06 - 11 = 5.0044498)
074 06-12=6.137    Result:17 AW:680 AC:1000 (06 - 11 = 5.0046687)
091 06-12=6.138    Result:17 AW:680 AC:1000 (06 - 11 = 5.0166021)
041 06-12=6.139    Result:17 AW:680 AC:1000 (06 - 11 = 5.0045682)
075 06-12=6.140    Result:17 AW:680 AC:1000 (06 - 11 = 4.9986982)
019 06-12=6.144    Result:17 AW:680 AC:1000 (06 - 11 = 5.0041735)
022 06-12=6.148    Result:17 AW:680 AC:1000 (06 - 11 = 5.0040841)
045 06-12=6.150    Result:17 AW:680 AC:1000 (06 - 11 = 4.9983305)
028 06-12=6.151    Result:17 AW:680 AC:1000 (06 - 11 = 4.9984521)
099 06-12=6.162    Result:17 AW:680 AC:1000 (06 - 11 = 5.0053554)
071 06-12=6.162    Result:17 AW:680 AC:1000 (06 - 11 = 4.9977531)
023 06-12=6.163    Result:17 AW:680 AC:1000 (06 - 11 = 4.9985637)
012 06-12=6.181    Result:17 AW:680 AC:1000 (06 - 11 = 4.9979702)
097 06-12=6.164    Result:17 AW:680 AC:1000 (06 - 11 = 4.9978511)
006 06-12=6.164    Result:17 AW:680 AC:1000 (06 - 11 = 5.0049827)
000 06-12=6.183    Result:17 AW:680 AC:1000 (06 - 11 = 4.9981968)
087 06-12=6.165    Result:17 AW:680 AC:1000 (06 - 11 = 5.0043538)
094 06-12=6.171    Result:17 AW:680 AC:1000 (06 - 11 = 4.9980719)
096 06-12=6.190    Result:17 AW:680 AC:1000 (06 - 11 = 4.9908075)
073 06-12=6.171    Result:17 AW:680 AC:1000 (06 - 11 = 4.9921006)
037 06-12=6.172    Result:17 AW:680 AC:1000 (06 - 11 = 4.9912101)
032 06-12=6.176    Result:17 AW:680 AC:1000 (06 - 11 = 4.9911085)
047 06-12=6.180    Result:17 AW:680 AC:1000 (06 - 11 = 4.9918838)
042 06-12=6.182    Result:17 AW:680 AC:1000 (06 - 11 = 4.9910048)
015 06-12=6.182    Result:17 AW:680 AC:1000 (06 - 11 = 4.991315)
038 06-12=6.183    Result:17 AW:680 AC:1000 (06 - 11 = 4.9909052)
090 06-12=6.183    Result:17 AW:680 AC:1000 (06 - 11 = 5.0174983)
057 06-12=6.184    Result:17 AW:680 AC:1000 (06 - 11 = 5.0176321)
044 06-12=6.184    Result:17 AW:680 AC:1000 (06 - 11 = 4.9922415)
046 06-12=6.186    Result:17 AW:680 AC:1000 (06 - 11 = 4.9895741)
031 06-12=6.195    Result:17 AW:680 AC:1000 (06 - 11 = 4.9975527)
040 06-12=6.195    Result:17 AW:680 AC:1000 (06 - 11 = 4.9919927)
004 06-12=6.195    Result:17 AW:680 AC:1000 (06 - 11 = 4.9914206)
036 06-12=6.214    Result:17 AW:680 AC:1000 (06 - 11 = 4.9896935)
069 06-12=6.196    Result:17 AW:680 AC:1000 (06 - 11 = 4.997648)

6277 ms
Press any key for continuing...
```

### .NET Core - Parallel.For 平行呼叫遠端非同步

在 .NET Core 專案內，修正 APIEndPoint 使用 `$"https://businessblazor.azurewebsites.net/api/RemoteService/AddAsync/8/9/{cost}";` 字串參數，這裡將會使用 Parallel.For 平行呼叫遠端的非同步 API

使用底下的程式碼，平行呼叫遠端非同步 Web API 100 次，每次呼叫 Web API 將會 5 秒鐘後才會得到執行結果

```csharp
int cost = 5000;
int MaxTasks = 100;
```

按下 [Ctrl] + [F5] ，使用 [啟動但不偵錯] 方式來執行這個專案

這裡是執行結果

```
061 32-38=6.006    Result:17 AW:680 AC:1000 (32 - 37 = 4.9979282)
012 32-38=6.093    Result:17 AW:680 AC:1000 (32 - 37 = 4.9960928)
024 32-38=6.093    Result:17 AW:680 AC:1000 (32 - 37 = 4.9937875)
030 32-38=6.004    Result:17 AW:680 AC:1000 (32 - 37 = 4.993988)
013 32-38=6.006    Result:17 AW:680 AC:1000 (32 - 37 = 4.993568)
043 32-38=6.024    Result:17 AW:680 AC:1000 (32 - 37 = 4.9974229)
029 32-38=6.004    Result:17 AW:680 AC:1000 (32 - 37 = 4.9975334)
000 32-38=6.093    Result:17 AW:680 AC:1000 (32 - 37 = 4.9921224)
036 32-38=6.093    Result:17 AW:680 AC:1000 (32 - 37 = 4.9924874)
062 32-38=6.005    Result:17 AW:680 AC:1000 (32 - 37 = 4.9981592)
017 32-38=6.005    Result:17 AW:680 AC:1000 (32 - 37 = 4.9942151)
039 32-38=6.025    Result:17 AW:680 AC:1000 (32 - 37 = 5.0070352)
096 32-38=6.112    Result:17 AW:680 AC:1000 (32 - 37 = 4.9972947)
084 32-38=6.112    Result:17 AW:680 AC:1000 (32 - 37 = 5.0024602)
044 32-38=6.024    Result:17 AW:680 AC:1000 (32 - 37 = 4.9968122)
093 32-38=6.024    Result:17 AW:680 AC:1000 (32 - 37 = 5.0023287)
059 32-38=6.024    Result:17 AW:680 AC:1000 (32 - 37 = 5.0051437)
046 32-38=6.025    Result:17 AW:680 AC:1000 (32 - 37 = 5.0045682)
055 32-38=6.025    Result:17 AW:680 AC:1000 (32 - 37 = 5.0043033)
067 32-38=6.022    Result:17 AW:680 AC:1000 (32 - 37 = 4.9976882)
035 32-38=6.024    Result:17 AW:680 AC:1000 (32 - 37 = 5.0049341)
007 32-38=6.024    Result:17 AW:680 AC:1000 (32 - 37 = 4.9966456)
022 32-38=6.024    Result:17 AW:680 AC:1000 (32 - 37 = 5.0050515)
004 32-38=6.025    Result:17 AW:680 AC:1000 (32 - 37 = 5.0053932)
075 32-38=6.026    Result:17 AW:680 AC:1000 (32 - 37 = 5.0044119)
097 32-38=6.026    Result:17 AW:680 AC:1000 (32 - 37 = 4.9965222)
002 32-38=6.024    Result:17 AW:680 AC:1000 (32 - 37 = 4.9969503)
077 32-38=6.025    Result:17 AW:680 AC:1000 (32 - 37 = 5.0057399)
088 32-38=6.025    Result:17 AW:680 AC:1000 (32 - 37 = 5.0025755)
040 32-38=6.026    Result:17 AW:680 AC:1000 (32 - 37 = 5.0055087)
001 32-38=6.027    Result:17 AW:680 AC:1000 (32 - 37 = 5.0052789)
086 32-38=6.027    Result:17 AW:680 AC:1000 (32 - 37 = 5.0048179)
081 32-38=6.023    Result:17 AW:680 AC:1000 (32 - 37 = 4.9970711)
091 32-38=6.026    Result:17 AW:680 AC:1000 (32 - 37 = 4.9991298)
092 32-38=6.026    Result:17 AW:680 AC:1000 (32 - 37 = 4.9985984)
027 32-38=6.026    Result:17 AW:680 AC:1000 (32 - 37 = 4.9987614)
053 32-38=6.026    Result:17 AW:680 AC:1000 (32 - 37 = 4.9979222)
064 32-38=6.025    Result:17 AW:680 AC:1000 (32 - 37 = 4.9984448)
079 32-38=6.025    Result:17 AW:680 AC:1000 (32 - 37 = 4.9977942)
060 32-38=6.115    Result:17 AW:680 AC:1000 (32 - 37 = 4.9971808)
023 32-38=6.026    Result:17 AW:680 AC:1000 (32 - 37 = 4.9974257)
083 32-38=6.026    Result:17 AW:680 AC:1000 (32 - 37 = 4.9980387)
048 32-38=6.117    Result:17 AW:680 AC:1000 (32 - 37 = 4.9975068)
014 32-38=6.030    Result:17 AW:680 AC:1000 (32 - 37 = 4.9983167)
041 32-38=6.030    Result:17 AW:680 AC:1000 (32 - 37 = 4.9967491)
085 32-38=6.032    Result:17 AW:680 AC:1000 (32 - 37 = 4.9989145)
063 32-38=6.030    Result:17 AW:680 AC:1000 (32 - 37 = 4.9963294)
045 32-38=6.032    Result:17 AW:680 AC:1000 (32 - 37 = 4.9962127)
073 32-38=6.034    Result:17 AW:680 AC:1000 (32 - 37 = 4.9973004)
031 32-38=6.033    Result:17 AW:680 AC:1000 (32 - 37 = 4.9966176)
020 32-38=6.038    Result:17 AW:680 AC:1000 (32 - 37 = 4.9981547)
005 32-38=6.046    Result:17 AW:680 AC:1000 (32 - 37 = 4.9959881)
090 32-38=6.049    Result:17 AW:680 AC:1000 (32 - 37 = 4.9976431)
015 32-38=6.050    Result:17 AW:680 AC:1000 (32 - 37 = 4.9953788)
057 32-38=6.049    Result:17 AW:680 AC:1000 (32 - 37 = 4.9952659)
037 32-38=6.066    Result:17 AW:680 AC:1000 (32 - 37 = 4.9945519)
025 32-38=6.059    Result:17 AW:680 AC:1000 (32 - 37 = 4.9956225)
010 32-38=6.057    Result:17 AW:680 AC:1000 (32 - 37 = 4.9961029)
034 32-38=6.057    Result:17 AW:680 AC:1000 (32 - 37 = 4.9955067)
018 32-38=6.059    Result:17 AW:680 AC:1000 (32 - 37 = 4.9957381)
058 32-38=6.058    Result:17 AW:680 AC:1000 (32 - 37 = 4.994454)
068 32-38=6.058    Result:17 AW:680 AC:1000 (32 - 37 = 4.9949902)
003 32-38=6.065    Result:17 AW:680 AC:1000 (32 - 37 = 4.9964723)
066 32-38=6.056    Result:17 AW:680 AC:1000 (32 - 37 = 4.9993611)
094 32-38=6.065    Result:17 AW:680 AC:1000 (32 - 37 = 4.9946875)
099 32-38=6.065    Result:17 AW:680 AC:1000 (32 - 37 = 4.9948474)
016 32-38=6.067    Result:17 AW:680 AC:1000 (32 - 37 = 4.9993059)
072 32-38=6.166    Result:17 AW:680 AC:1000 (32 - 37 = 4.995872)
051 32-38=6.079    Result:17 AW:680 AC:1000 (32 - 37 = 4.9951152)
032 32-38=6.080    Result:17 AW:680 AC:1000 (32 - 37 = 4.9943492)
047 32-38=6.081    Result:17 AW:680 AC:1000 (32 - 37 = 4.9986237)
089 32-38=6.082    Result:17 AW:680 AC:1000 (32 - 37 = 4.9991901)
042 32-38=6.082    Result:17 AW:680 AC:1000 (32 - 37 = 4.9992369)
078 32-38=6.082    Result:17 AW:680 AC:1000 (32 - 37 = 4.9987391)
038 32-38=6.085    Result:17 AW:680 AC:1000 (32 - 37 = 4.9989634)
006 32-38=6.090    Result:17 AW:680 AC:1000 (32 - 37 = 4.9990837)
070 32-38=6.089    Result:17 AW:680 AC:1000 (32 - 37 = 4.9988501)
021 32-38=6.090    Result:17 AW:681 AC:1000 (32 - 37 = 5.0052344)
054 32-38=6.073    Result:17 AW:681 AC:1000 (32 - 37 = 5.0054467)
009 32-38=6.092    Result:17 AW:681 AC:1000 (32 - 37 = 5.005346)
074 32-38=6.093    Result:17 AW:681 AC:1000 (32 - 37 = 5.0055749)
069 32-38=6.091    Result:17 AW:681 AC:1000 (32 - 37 = 5.0011222)
065 32-38=6.093    Result:17 AW:681 AC:1000 (32 - 37 = 5.0008488)
056 32-38=6.093    Result:17 AW:681 AC:1000 (32 - 37 = 5.0050374)
082 32-38=6.093    Result:17 AW:681 AC:1000 (32 - 37 = 5.0001916)
080 32-38=6.094    Result:17 AW:681 AC:1000 (32 - 37 = 5.0004746)
087 32-38=6.096    Result:17 AW:681 AC:1000 (32 - 37 = 5.0006764)
028 32-38=6.096    Result:17 AW:681 AC:1000 (32 - 37 = 5.0047835)
008 32-38=6.096    Result:17 AW:681 AC:1000 (32 - 37 = 5.0051371)
076 32-38=6.096    Result:17 AW:681 AC:1000 (32 - 37 = 5.0009852)
052 32-38=6.098    Result:17 AW:681 AC:1000 (32 - 37 = 5.0046714)
050 32-38=6.098    Result:17 AW:681 AC:1000 (32 - 37 = 5.0049246)
049 32-38=6.099    Result:17 AW:681 AC:1000 (32 - 37 = 5.0057588)
095 32-38=6.100    Result:17 AW:681 AC:1000 (32 - 37 = 4.9999583)
098 32-38=6.110    Result:17 AW:681 AC:1000 (32 - 37 = 5.0003473)
033 32-38=6.110    Result:17 AW:681 AC:1000 (32 - 37 = 5.0000753)
026 32-38=6.111    Result:17 AW:681 AC:1000 (32 - 37 = 5.0057052)
019 32-40=7.380    Result:17 AW:680 AC:1000 (33 - 38 = 5.0098671)
011 32-40=7.382    Result:17 AW:680 AC:1000 (33 - 38 = 5.0102336)
071 32-40=7.426    Result:17 AW:680 AC:1000 (34 - 39 = 5.0115655)

7543 ms
Press any key for continuing...
```

### 在 .NET Framework 專案下，執行 Parallel.For 平行呼叫遠端同步

在 .NET Core 專案內，修正 APIEndPoint 使用 `$"https://businessblazor.azurewebsites.net/api/RemoteService/Add/8/9/{cost}";` 字串參數，這裡將會使用 Parallel.For 平行呼叫遠端的同步 API

使用底下的程式碼，平行呼叫遠端非同步 Web API 100 次，每次呼叫 Web API 將會 5 秒鐘後才會得到執行結果

```csharp
int cost = 5000;
int MaxTasks = 100;
```

按下 [Ctrl] + [F5] ，使用 [啟動但不偵錯] 方式來執行這個專案

這裡是執行結果 (請注意控制台出現文字的內容與時間序列)

```
047 34-47=13.081    Result:17 AW:677 AC:1000 (34 - 39 = 5.0071296)
084 34-47=13.104    Result:17 AW:675 AC:1000 (36 - 41 = 5.0107193)
069 34-47=13.083    Result:17 AW:677 AC:1000 (34 - 39 = 5.0052679)
045 34-47=13.084    Result:17 AW:672 AC:1000 (39 - 44 = 5.0114154)
002 34-47=13.084    Result:17 AW:672 AC:1000 (39 - 44 = 5.0064872)
053 34-47=13.084    Result:17 AW:672 AC:1000 (39 - 44 = 5.0131015)
023 34-47=13.085    Result:17 AW:672 AC:1000 (39 - 44 = 5.0108276)
021 34-47=13.084    Result:17 AW:673 AC:1000 (38 - 43 = 5.0056794)
022 34-47=13.085    Result:17 AW:672 AC:1000 (39 - 44 = 5.0116566)
012 34-47=13.109    Result:17 AW:672 AC:1000 (39 - 44 = 5.0122129)
000 34-47=13.112    Result:17 AW:679 AC:1000 (34 - 39 = 5.0025656)
008 34-47=13.086    Result:17 AW:676 AC:1000 (35 - 40 = 5.0133581)
058 34-47=13.085    Result:17 AW:674 AC:1000 (37 - 42 = 5.0131374)
076 34-47=13.085    Result:17 AW:671 AC:1000 (40 - 45 = 5.0055709)
016 34-47=13.085    Result:17 AW:678 AC:1000 (34 - 39 = 5.0080771)
096 34-47=13.107    Result:17 AW:677 AC:1000 (34 - 39 = 5.0044033)
057 34-47=13.088    Result:17 AW:671 AC:1000 (40 - 45 = 5.0058053)
065 34-47=13.085    Result:17 AW:670 AC:1000 (41 - 46 = 5.0014771)
052 34-01=27.102    Result:17 AW:667 AC:1000 (44 - 49 = 5.0044827)
075 34-01=27.102    Result:17 AW:670 AC:1000 (41 - 46 = 5.0015286)
017 34-01=27.102    Result:17 AW:668 AC:1000 (43 - 48 = 5.0053687)
046 34-01=27.102    Result:17 AW:667 AC:1000 (44 - 49 = 5.0051362)
038 34-01=27.102    Result:17 AW:667 AC:1000 (45 - 50 = 5.0018304)
068 34-01=27.102    Result:17 AW:667 AC:1000 (44 - 49 = 5.0090768)
024 34-01=27.122    Result:17 AW:667 AC:1000 (44 - 49 = 5.0065366)
006 34-01=27.102    Result:17 AW:667 AC:1000 (44 - 49 = 5.0059734)
072 34-01=27.126    Result:17 AW:667 AC:1000 (44 - 49 = 5.0070414)
003 34-01=27.103    Result:17 AW:668 AC:1000 (43 - 48 = 5.0066644)
030 34-01=27.106    Result:17 AW:667 AC:1000 (44 - 49 = 5.0103312)
039 34-01=27.106    Result:17 AW:663 AC:1000 (50 - 55 = 5.008382)
061 34-01=27.106    Result:17 AW:662 AC:1000 (51 - 56 = 5.0081407)
028 34-01=27.106    Result:17 AW:666 AC:1000 (46 - 51 = 5.0068291)
007 34-01=27.106    Result:17 AW:667 AC:1000 (45 - 50 = 5.0026153)
044 34-01=27.106    Result:17 AW:666 AC:1000 (46 - 51 = 5.0112945)
080 34-01=27.109    Result:17 AW:664 AC:1000 (49 - 54 = 5.0060206)
054 34-01=27.106    Result:17 AW:665 AC:1000 (47 - 52 = 5.0109673)
004 34-01=27.106    Result:17 AW:665 AC:1000 (47 - 52 = 5.0074027)
093 34-01=27.107    Result:17 AW:664 AC:1000 (49 - 54 = 5.0062241)
051 34-01=27.111    Result:17 AW:665 AC:1000 (47 - 52 = 5.0078599)
036 34-01=27.129    Result:17 AW:664 AC:1000 (49 - 54 = 5.0046946)
088 34-01=27.107    Result:17 AW:666 AC:1000 (46 - 51 = 5.0059203)
035 34-01=27.114    Result:17 AW:664 AC:1000 (49 - 54 = 5.013166)
060 34-01=27.140    Result:17 AW:661 AC:1000 (52 - 57 = 5.0057622)
041 34-01=27.107    Result:17 AW:665 AC:1000 (48 - 53 = 5.0065041)
005 34-01=27.109    Result:17 AW:664 AC:1000 (49 - 54 = 5.0126286)
092 34-01=27.117    Result:17 AW:662 AC:1000 (51 - 56 = 5.0071555)
050 34-01=27.123    Result:17 AW:661 AC:1000 (52 - 57 = 5.0042532)
062 34-01=27.124    Result:17 AW:663 AC:1000 (50 - 55 = 5.0110622)
056 34-01=27.112    Result:17 AW:665 AC:1000 (48 - 53 = 5.0056682)
066 34-01=27.116    Result:17 AW:664 AC:1000 (49 - 54 = 5.0118428)
079 34-01=27.116    Result:17 AW:663 AC:1000 (50 - 55 = 5.0131411)
027 34-01=27.115    Result:17 AW:664 AC:1000 (49 - 54 = 5.00554)
077 34-01=27.122    Result:17 AW:662 AC:1000 (51 - 56 = 5.0075115)
011 34-01=27.121    Result:17 AW:660 AC:1000 (53 - 58 = 5.0123625)
071 34-01=27.124    Result:17 AW:661 AC:1000 (52 - 57 = 5.0060246)
037 34-01=27.120    Result:17 AW:662 AC:1000 (51 - 56 = 5.0073804)
020 34-01=27.124    Result:17 AW:660 AC:1000 (53 - 58 = 5.0124185)
063 34-01=27.128    Result:17 AW:659 AC:1000 (54 - 59 = 5.0020069)
086 34-01=27.126    Result:17 AW:659 AC:1000 (54 - 59 = 5.0030995)
026 34-01=27.137    Result:17 AW:664 AC:1000 (49 - 54 = 5.013826)
085 34-01=27.155    Result:17 AW:660 AC:1000 (53 - 58 = 5.0131813)
064 34-01=27.156    Result:17 AW:659 AC:1000 (54 - 59 = 5.0029775)
029 34-01=27.172    Result:17 AW:659 AC:1000 (54 - 59 = 5.0114089)
091 34-01=27.174    Result:17 AW:659 AC:1000 (54 - 59 = 5.0022332)
042 34-01=27.190    Result:17 AW:659 AC:1000 (54 - 59 = 5.0105673)
097 34-01=27.192    Result:17 AW:661 AC:1000 (52 - 57 = 5.0036127)
009 34-01=27.236    Result:17 AW:658 AC:1000 (55 - 00 = 5.0071325)
073 34-01=27.238    Result:17 AW:658 AC:1000 (55 - 00 = 5.0077044)
059 34-01=27.248    Result:17 AW:658 AC:1000 (55 - 00 = 5.0182073)
001 34-01=27.247    Result:17 AW:669 AC:1000 (42 - 47 = 5.0054308)
090 34-01=27.245    Result:17 AW:659 AC:1000 (54 - 59 = 5.0125477)
031 34-01=27.245    Result:17 AW:669 AC:1000 (42 - 47 = 5.0040128)
043 34-01=27.246    Result:17 AW:659 AC:1000 (54 - 59 = 5.0119985)
099 34-01=27.246    Result:17 AW:658 AC:1000 (55 - 00 = 5.0027934)
094 34-01=27.247    Result:17 AW:659 AC:1000 (54 - 59 = 5.0023549)
010 34-02=28.079    Result:17 AW:657 AC:1000 (56 - 01 = 5.0109936)
025 34-02=28.082    Result:17 AW:657 AC:1000 (56 - 01 = 5.0099873)
049 34-02=28.095    Result:17 AW:657 AC:1000 (56 - 01 = 5.0102597)
098 34-02=28.096    Result:17 AW:657 AC:1000 (56 - 01 = 5.0107876)
095 34-02=28.110    Result:17 AW:657 AC:1000 (56 - 01 = 5.0107686)
015 34-03=29.066    Result:17 AW:657 AC:1000 (57 - 02 = 5.0007155)
070 34-03=29.082    Result:17 AW:657 AC:1000 (57 - 02 = 5.0007384)
034 34-03=29.083    Result:17 AW:657 AC:1000 (57 - 02 = 5.0013743)
081 34-03=29.143    Result:17 AW:657 AC:1000 (57 - 02 = 5.0007057)
087 34-04=30.066    Result:17 AW:656 AC:1000 (58 - 03 = 5.0053062)
074 34-04=30.081    Result:17 AW:656 AC:1000 (58 - 03 = 5.0061148)
040 34-04=30.097    Result:17 AW:656 AC:1000 (58 - 03 = 5.0057439)
048 34-04=30.121    Result:17 AW:656 AC:1000 (58 - 03 = 5.0064884)
018 34-05=31.079    Result:17 AW:655 AC:1000 (59 - 04 = 5.0111059)
033 34-05=31.079    Result:17 AW:655 AC:1000 (59 - 04 = 5.0119696)
083 34-05=31.082    Result:17 AW:655 AC:1000 (59 - 04 = 5.0123626)
014 34-05=31.094    Result:17 AW:655 AC:1000 (59 - 04 = 5.012031)
089 34-05=31.095    Result:17 AW:655 AC:1000 (59 - 04 = 5.0126403)
082 34-05=31.141    Result:17 AW:655 AC:1000 (59 - 04 = 5.0122107)
078 34-05=31.227    Result:17 AW:655 AC:1000 (59 - 04 = 5.0107799)
019 34-05=31.227    Result:17 AW:655 AC:1000 (59 - 04 = 5.0183919)
067 34-05=31.228    Result:17 AW:655 AC:1000 (59 - 04 = 5.0103717)
032 34-05=31.227    Result:17 AW:655 AC:1000 (59 - 04 = 5.0097591)
013 34-06=32.074    Result:17 AW:655 AC:1000 (00 - 05 = 5.0052974)
055 34-06=32.090    Result:17 AW:655 AC:1000 (00 - 05 = 5.0058578)

32152 ms
Press any key for continuing...
```


### 改善在 .NET Framework 專案下，執行 Parallel.For 平行呼叫遠端同步的執行產出量

在 .NET Core 專案內，修正 APIEndPoint 使用 `$"https://businessblazor.azurewebsites.net/api/RemoteService/Add/8/9/{cost}";` 字串參數，這裡將會使用 Parallel.For 平行呼叫遠端的同步 API

使用底下的程式碼，平行呼叫遠端非同步 Web API 100 次，每次呼叫 Web API 將會 5 秒鐘後才會得到執行結果

```csharp
int cost = 5000;
int MaxTasks = 100;
```

在瀏覽器上輸入與開啟這個 URL [https://businessblazor.azurewebsites.net/api/RemoteService/GetThreadPool](https://businessblazor.azurewebsites.net/api/RemoteService/GetThreadPool) ， 將會得到底下的輸出內容

```
AW:679 AC:1000 MaxW:682 MaxC:1000 MinW:1 MinC:1 
```

在瀏覽器上輸入與開啟這個 URL [https://businessblazor.azurewebsites.net/api/RemoteService/SetThreadPool/120/120/](https://businessblazor.azurewebsites.net/api/RemoteService/SetThreadPool/120/120/) ， 將會得到底下的輸出內容

```
OK AW:680 AC:1000 MaxW:682 MaxC:1000 MinW:120 MinC:120 
```

按下 [Ctrl] + [F5] ，使用 [啟動但不偵錯] 方式來執行這個專案

這裡是執行結果 (請注意控制台出現文字的內容與時間序列)

```
038 59-05=6.022    Result:17 AW:678 AC:1000 (59 - 04 = 5.0002921)
060 59-05=6.043    Result:17 AW:679 AC:1000 (59 - 04 = 5.002058)
036 59-05=6.043    Result:17 AW:680 AC:1000 (59 - 04 = 5.0054472)
079 59-05=6.042    Result:17 AW:670 AC:1000 (59 - 04 = 5.0059578)
037 59-05=6.042    Result:17 AW:673 AC:1000 (59 - 04 = 5.0131215)
027 59-05=6.043    Result:17 AW:674 AC:1000 (59 - 04 = 5.0135103)
083 59-05=6.044    Result:17 AW:675 AC:1000 (59 - 04 = 5.0153631)
062 59-05=6.044    Result:17 AW:676 AC:1000 (59 - 04 = 5.0162388)
014 59-05=6.045    Result:17 AW:668 AC:1000 (59 - 04 = 5.0054237)
095 59-05=6.044    Result:17 AW:669 AC:1000 (59 - 04 = 5.0081958)
017 59-05=6.045    Result:17 AW:667 AC:1000 (59 - 04 = 5.0048636)
020 59-05=6.046    Result:17 AW:677 AC:1000 (59 - 04 = 5.016984)
081 59-05=6.046    Result:17 AW:671 AC:1000 (59 - 04 = 5.0112869)
045 59-05=6.047    Result:17 AW:672 AC:1000 (59 - 04 = 5.0116391)
030 59-05=6.058    Result:17 AW:652 AC:1000 (59 - 04 = 5.0116894)
000 59-05=6.079    Result:17 AW:654 AC:1000 (59 - 04 = 5.0123499)
058 59-05=6.059    Result:17 AW:649 AC:1000 (59 - 04 = 5.0028745)
078 59-05=6.060    Result:17 AW:655 AC:1000 (59 - 04 = 5.0126415)
094 59-05=6.059    Result:17 AW:653 AC:1000 (59 - 04 = 5.0121125)
092 59-05=6.059    Result:17 AW:659 AC:1000 (59 - 04 = 5.0136216)
046 59-05=6.060    Result:17 AW:656 AC:1000 (59 - 04 = 5.0128903)
023 59-05=6.061    Result:17 AW:660 AC:1000 (59 - 04 = 5.0138597)
051 59-05=6.061    Result:17 AW:650 AC:1000 (59 - 04 = 5.0084795)
063 59-05=6.062    Result:17 AW:665 AC:1000 (59 - 04 = 5.0159813)
022 59-05=6.061    Result:17 AW:657 AC:1000 (59 - 04 = 5.0131293)
012 59-05=6.081    Result:17 AW:661 AC:1000 (59 - 04 = 5.0141517)
096 59-05=6.081    Result:17 AW:662 AC:1000 (59 - 04 = 5.0143927)
084 59-05=6.083    Result:17 AW:663 AC:1000 (59 - 04 = 5.0146804)
047 59-05=6.063    Result:17 AW:664 AC:1000 (59 - 04 = 5.0152022)
040 59-05=6.063    Result:17 AW:651 AC:1000 (59 - 04 = 5.0089723)
059 59-05=6.062    Result:17 AW:666 AC:1000 (59 - 04 = 5.0163037)
018 59-05=6.061    Result:17 AW:658 AC:1000 (59 - 04 = 5.0133943)
056 59-05=6.069    Result:17 AW:648 AC:1000 (59 - 04 = 5.0081735)
002 59-05=6.098    Result:17 AW:608 AC:1000 (59 - 04 = 5.0028971)
032 59-05=6.098    Result:17 AW:603 AC:1000 (59 - 04 = 5.0003173)
085 59-05=6.099    Result:17 AW:613 AC:1000 (59 - 04 = 5.0043482)
072 59-05=6.121    Result:17 AW:618 AC:1000 (59 - 04 = 5.0057147)
071 59-05=6.099    Result:17 AW:621 AC:1000 (59 - 04 = 5.0064065)
004 59-05=6.101    Result:17 AW:615 AC:1000 (59 - 04 = 5.004934)
007 59-05=6.098    Result:17 AW:606 AC:1000 (59 - 04 = 5.0021226)
075 59-05=6.101    Result:17 AW:625 AC:1000 (59 - 04 = 5.0075617)
028 59-05=6.101    Result:17 AW:626 AC:1000 (59 - 04 = 5.0078282)
077 59-05=6.101    Result:17 AW:614 AC:1000 (59 - 04 = 5.0046049)
013 59-05=6.113    Result:17 AW:624 AC:1000 (59 - 04 = 5.0071175)
011 59-05=6.111    Result:17 AW:612 AC:1000 (59 - 04 = 5.0040977)
090 59-05=6.111    Result:17 AW:627 AC:1000 (59 - 04 = 5.0081085)
008 59-05=6.111    Result:17 AW:617 AC:1000 (59 - 04 = 5.0054424)
019 59-05=6.112    Result:17 AW:622 AC:1000 (59 - 04 = 5.0066338)
087 59-05=6.114    Result:17 AW:616 AC:1000 (59 - 04 = 5.0051782)
033 59-05=6.115    Result:17 AW:607 AC:1000 (59 - 04 = 5.0024017)
086 59-05=6.113    Result:17 AW:623 AC:1000 (59 - 04 = 5.0068757)
055 59-05=6.116    Result:17 AW:632 AC:1000 (59 - 04 = 5.0104172)
069 59-05=6.120    Result:17 AW:635 AC:1000 (59 - 04 = 5.0113912)
016 59-05=6.125    Result:17 AW:611 AC:1000 (59 - 04 = 5.0037352)
029 59-05=6.128    Result:17 AW:609 AC:1000 (59 - 04 = 5.0031435)
025 59-05=6.128    Result:17 AW:634 AC:1000 (59 - 04 = 5.0110465)
026 59-05=6.131    Result:17 AW:629 AC:1000 (59 - 04 = 5.0091717)
098 59-05=6.130    Result:17 AW:639 AC:1000 (59 - 04 = 5.0164213)
066 59-05=6.131    Result:17 AW:610 AC:1000 (59 - 04 = 5.003399)
088 59-05=6.132    Result:17 AW:640 AC:1000 (59 - 04 = 5.0166674)
001 59-05=6.132    Result:17 AW:633 AC:1000 (59 - 04 = 5.0107391)
015 59-05=6.142    Result:17 AW:636 AC:1000 (59 - 04 = 5.0116482)
009 59-05=6.144    Result:17 AW:638 AC:1000 (59 - 04 = 5.0123548)
005 59-05=6.144    Result:17 AW:630 AC:1000 (59 - 04 = 5.0096024)
061 59-05=6.147    Result:17 AW:619 AC:1000 (59 - 04 = 5.0059595)
003 59-05=6.147    Result:17 AW:631 AC:1000 (59 - 04 = 5.0100054)
073 59-05=6.148    Result:17 AW:647 AC:1000 (59 - 04 = 5.018663)
082 59-05=6.147    Result:17 AW:620 AC:1000 (59 - 04 = 5.00617)
091 59-05=6.148    Result:17 AW:645 AC:1000 (59 - 04 = 5.0179952)
049 59-05=6.149    Result:17 AW:642 AC:1000 (59 - 04 = 5.0172136)
010 59-05=6.152    Result:17 AW:637 AC:1000 (59 - 04 = 5.0120809)
080 59-05=6.157    Result:17 AW:643 AC:1000 (59 - 04 = 5.0174564)
076 59-05=6.160    Result:17 AW:628 AC:1000 (59 - 04 = 5.0088168)
050 59-05=6.163    Result:17 AW:605 AC:1000 (59 - 04 = 5.0017606)
006 59-05=6.163    Result:17 AW:644 AC:1000 (59 - 04 = 5.0177232)
048 59-05=6.184    Result:17 AW:604 AC:1000 (59 - 04 = 5.0008225)
021 59-05=6.164    Result:17 AW:641 AC:1000 (59 - 04 = 5.0169181)
097 59-05=6.167    Result:17 AW:601 AC:1000 (59 - 04 = 5.0218567)
024 59-05=6.185    Result:17 AW:646 AC:1000 (59 - 04 = 5.0182597)
034 59-05=6.167    Result:17 AW:584 AC:1000 (59 - 04 = 5.0199074)
070 59-05=6.168    Result:17 AW:581 AC:1000 (59 - 04 = 5.0189957)
035 59-05=6.175    Result:17 AW:587 AC:1000 (59 - 04 = 5.0206798)
089 59-05=6.178    Result:17 AW:602 AC:1000 (59 - 04 = 5.0220933)
052 59-05=6.182    Result:17 AW:588 AC:1000 (59 - 04 = 5.0209513)
031 59-05=6.181    Result:17 AW:583 AC:1000 (59 - 04 = 5.0196217)
054 59-05=6.181    Result:17 AW:585 AC:1000 (59 - 04 = 5.0201418)
042 59-05=6.183    Result:17 AW:590 AC:1000 (59 - 04 = 5.0214244)
099 59-05=6.182    Result:17 AW:598 AC:1000 (59 - 04 = 5.0208981)
074 59-05=6.195    Result:17 AW:600 AC:1000 (59 - 04 = 5.0215863)
064 59-05=6.192    Result:17 AW:593 AC:1000 (59 - 04 = 5.022153)
065 59-05=6.193    Result:17 AW:594 AC:1000 (59 - 04 = 5.0224114)
057 59-05=6.198    Result:17 AW:596 AC:1000 (59 - 04 = 5.0231321)
044 59-05=6.193    Result:17 AW:595 AC:1000 (59 - 04 = 5.0229177)
067 59-05=6.192    Result:17 AW:586 AC:1000 (59 - 04 = 5.0204238)
068 59-05=6.192    Result:17 AW:597 AC:1000 (59 - 04 = 5.0233678)
093 59-05=6.193    Result:17 AW:599 AC:1000 (59 - 04 = 5.0213357)
041 59-05=6.194    Result:17 AW:589 AC:1000 (59 - 04 = 5.0212076)
043 59-05=6.196    Result:17 AW:592 AC:1000 (59 - 04 = 5.0219009)
053 59-05=6.198    Result:17 AW:591 AC:1000 (59 - 04 = 5.0216581)
039 59-05=6.200    Result:17 AW:582 AC:1000 (59 - 04 = 5.0193887)

6273 ms
Press any key for continuing...
```
