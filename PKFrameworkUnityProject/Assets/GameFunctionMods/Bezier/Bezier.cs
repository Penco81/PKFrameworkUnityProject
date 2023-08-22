using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier
{
    public List<BezierNode> bezierNodes;
    public int accuracy = 10;
    
    
    public void SetBezierNode(List<BezierNodeObject> bezierNodeObjects) {
        if (bezierNodes == null) 
            bezierNodes = new List<BezierNode>();
        bezierNodes.Clear();

        foreach (var item in bezierNodeObjects)
        {
            bezierNodes.Add(item.GetBezierNode());
        }
    }
    
    /// <summary>
    /// 计算并返回指定一段曲线的坐标位置数组
    /// </summary>
    /// <param name="region">区间下标数值</param>
    /// <returns></returns>
    public Vector3[] GetBezierDatas(int region) {
        if (region < bezierNodes.Count) 
        {
            Vector3[] datas = new Vector3[accuracy];
            for (int i = 0; i < accuracy; i++)
            {
                datas[i] = BezierMath.Bezier_3(
                    bezierNodes[region].nodePos,
                    bezierNodes[region].getReverseNodeOffset(),
                    bezierNodes[region + 1].nodeOffset,
                    bezierNodes[region + 1].nodePos,
                    i/(accuracy-1.0f)
                );
            }
            return datas;
        }
        return null;
    }
}
