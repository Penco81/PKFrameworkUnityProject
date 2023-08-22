using UnityEngine;

/// <summary>
/// 抛物线导弹
/// <para>计算弹道和转向</para>
/// <para>ZhangYu 2019-02-27</para>
/// </summary>
public class Missile : MonoBehaviour {

    public Transform target;        // 目标
    public float height = 16f;      // 高度
    public float gravity = -9.8f;   // 重力加速度
    private ParabolaPath path;      // 抛物线运动轨迹

    private void Start() {
        path = new ParabolaPath(transform.position, target.position, height, gravity);
        path.isClampStartEnd = true;
        transform.LookAt(path.GetPosition(path.time + Time.deltaTime));
    }

    private void Update() {
        // 计算位移
        float t = Time.deltaTime;
        path.time += t;
        transform.position = path.position;

        // 计算转向
        transform.LookAt(path.GetPosition(path.time + t));

        // 简单模拟一下碰撞检测
        if (path.time >= path.totalTime) enabled = false;
    }

}