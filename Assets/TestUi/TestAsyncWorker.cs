using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using dotnet.Threading;

public struct MyTask
{
    public int Tp { get; set; }
    public int Num { get; set; }
}
public class TestAsyncWorker : MonoBehaviour
{
    private bool isBegin = false;
    private AsyncQueue<MyTask> sq = new AsyncQueue<MyTask>();
    private Thread sockThread;
    private Thread mainTread;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("==KeyDown A==");
            if (!isBegin)
            {   
                // Debug.Log("==test step 1==");
                // test(); //测试异步aysnc await
                // Debug.Log("==test step 4==");
                isBegin = true;
                sockThread = new Thread(sockThreadFun);
                sockThread.Start();
                mainTread = new Thread(mainThreadFun);
                mainTread.Start();
            }
        }
    }
    private void sockThreadFun()
    {
        int i = 0;
        while (true)
        {
            ++i;
            sq.Enqueue(new MyTask(){Tp = 1, Num = i,});
            Debug.Log("sockThreadFunc do==" + i);
            Thread.Sleep(2000);
        }
    }
    private void mainThreadFun()
    {
        while (true)
        {
            var ret = sq.DequeueAsync();
            var mtask = ret.Result;
            Debug.Log("mainThreadFunc do==Tp= "+mtask.Tp + " Num=" +mtask.Num);
        }
    }
    private async Task<int> test()
    {
        Debug.Log("==test step 2==");
        int r = await Task<int>.Run(() =>
        {
            Thread.Sleep(3000);
            return 100;
        });
        Debug.Log("==test step 3==");
        return r;
    }
}
