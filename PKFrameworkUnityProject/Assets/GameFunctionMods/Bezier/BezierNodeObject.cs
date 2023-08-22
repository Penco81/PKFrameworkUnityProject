using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BezierNode
{
    //节点的位置
    public Vector3 nodePos;
    //控制节点的位置
    public Vector3 nodeOffset;
    public Vector3 getReverseNodeOffset() {
        return nodePos - (nodeOffset - nodePos);
    }
}

public class BezierNodeObject : MonoBehaviour
{
    public Transform BezierOffset;

    private BezierNode bezierNode;

    public BezierNode GetBezierNode()
    {
        bezierNode.nodeOffset = BezierOffset.position;
        bezierNode.nodePos = transform.position;
        return bezierNode;
    }

    /// <summary>
    /// 绘制出当前节点的控制节点
    /// </summary>
    private void Update()
    {
        Debug.DrawLine(BezierOffset.position, transform.position - (BezierOffset.position - transform.position), Color.yellow);
    }
}