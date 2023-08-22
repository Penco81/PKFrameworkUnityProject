using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//reference：https://blog.csdn.net/linxinfa/article/details/116808549
//reference: https://www.jianshu.com/p/b675688618ef?utm_campaign=maleskine&utm_content=note&utm_medium=seo_notes&utm_source=recommendation
//一阶贝塞尔曲线公式：B(t) = P1 + (P2 − P1)t = P1(1−t)+ P2t, t∈[0,1]
//二阶：B(t) = P1(1 - t)2 + 2P2t(1 - t) + P3t2，t∈[0,1]
//二阶推导：
// M(t) = P1(1 - t) + P2t
// N(t) = P2(1 - t) + P3t
// B(t) = M(1 - t) + Nt
// 代入：
// B(t) = (1 - t)( (1 - t)P1 + tP2) + t((1 - t)P2 + tP3)
// 化简：
// B(t) = P1 - 2P1t + P1t2 + 2P2t - 2P1t2 + P3t3
// 整理：
// B(t) = P1(1 - t)2 + 2P2t(1 - t) + P3t2

public class BezierMath
{
    /// <summary>
    /// 一次贝塞尔
    /// </summary>
    public static Vector3 Bezier_1(Vector3 p0, Vector3 p1, float t)
    {
        return (1 - t) * p0 + p1 * t;
    }

    /// <summary>
    /// 二次贝塞尔
    /// </summary>
    public static Vector3 Bezier_2(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return (1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2);
    }

    /// <summary>
    /// 三次贝塞尔
    /// </summary>
    public static Vector3 Bezier_3(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return (1 - t) * ((1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2)) + t * ((1 - t) * ((1 - t) * p1 + t * p2) + t * ((1 - t) * p2 + t * p3));
    }
}
