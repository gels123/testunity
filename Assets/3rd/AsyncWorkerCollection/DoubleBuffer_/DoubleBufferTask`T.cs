﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dotnet.Threading
{
    /// <summary>
    /// 双缓存任务
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if PublicAsInternal
    internal
#else
    public
#endif
    class DoubleBufferTask<T> : DoubleBufferTask<List<T>, T>
    {
        /// <summary>
        /// 创建双缓存任务，执行任务的方法放在 <paramref name="doTask"/> 方法
        /// </summary>
        /// <param name="doTask"></param>
        public DoubleBufferTask(Func<List<T>, Task> doTask) : base(new List<T>(), new List<T>(), doTask)
        {
        }
    }
}
