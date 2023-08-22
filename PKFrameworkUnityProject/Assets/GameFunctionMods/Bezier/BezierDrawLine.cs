using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierDrawLine : MonoBehaviour
{
    private List<BezierNodeObject> bezierNodeObjects;
    private Bezier bezierData;
    private List<Vector3> vector3s;

    public Transform NodesRoot;

    [Header("精度系数,表示每一段有多少个节点"),Range(10 , 100)]
    public int accuracy;

    private void Start()
    {
        bezierNodeObjects = new List<BezierNodeObject>();
        bezierData = new Bezier();
        vector3s = new List<Vector3>();
    }

    private void Update()
    {
        if (!NodesRoot) return;
        bezierNodeObjects.Clear();
        bezierNodeObjects.AddRange(NodesRoot.GetComponentsInChildren<BezierNodeObject>());
        bezierData.SetBezierNode(bezierNodeObjects);
        bezierData.accuracy = accuracy;
        drawline();
    }

    void drawline() {
        for (int i = 0; i < bezierNodeObjects.Count - 1; i++)
        {
            var lineRenderer = bezierNodeObjects[i].GetComponent<LineRenderer>();
            if (lineRenderer == null) lineRenderer = bezierNodeObjects[i].gameObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = accuracy;
            lineRenderer.SetPositions(bezierData.GetBezierDatas(i));
        }
    }
}