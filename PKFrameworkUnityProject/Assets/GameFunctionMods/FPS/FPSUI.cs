using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// FPS帧率UI
/// <para>ZhangYu 2018-02-10</para>
/// <para>blog:https://segmentfault.com/a/1190000020159916</para>
/// </summary>
public class FPSUI : MonoBehaviour {

    public Text text;               // 文本组件
    public float sampleTime = 0.5f; // 采样时间
    private int frame;              // 经过帧数
    private float time = 0;         // 运行时间

    private void Update () {
        frame += 1;
        time += Time.deltaTime;

        // 刷新帧率
        if (time >= sampleTime) {
            float fps = frame / time;
            text.text = "FPS:" + fps.ToString("F2");
            frame = 0;
            time = 0;
        }
    }
}