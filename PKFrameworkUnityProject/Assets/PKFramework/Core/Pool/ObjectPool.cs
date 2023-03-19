using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 通用型对象池
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObjectPool<T>
{

    const int m_defaultPoolSize = 10;
    private List<T> pool = new List<T>(m_defaultPoolSize); 
    private Func<T> func;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="func">委托 返回T类型</param>
    /// <param name="count">数目</param>
    public ObjectPool(Func<T> func,int count = m_defaultPoolSize)
    {
        this.func = func;
        InstanceObject(count);
    }
    /// <summary>
    /// 获取对象池中对象
    /// </summary>
    /// <returns></returns>
    public T GetObject()
    {
        int i = pool.Count;
        //当对象池数目大于0时直接移除
        while (i-->0)
        {
            T t = pool[i];
            pool.RemoveAt(i);
            return t;
        }
        //当对象池无对象时，生成对象 再递归 调用移除方法
        InstanceObject(3);
        return GetObject();
    }
    /// <summary>
    /// 添加对象
    /// </summary>
    /// <param name="t"></param>
    public void AddObject(T t)
    {
        pool.Add(t);
    }

    /// <summary>
    /// 实例化对象
    /// </summary>
    /// <param name="count"></param>
    public void InstanceObject(int count)
    {
        for (int i = 0; i < count; i++)
        {
            pool.Add(func());
        }
    }
}

