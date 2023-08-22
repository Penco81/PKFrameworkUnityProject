using PKFramework.Runtime.Singleton;
using UnityEngine;

namespace PKFramework.Runtime.Task
{
    public class TaskComponent : MonoSingleton<TaskComponent>
    {
        [SerializeField] [Tooltip("每个队列最大单帧更新数量。")]
        private byte maxRequests = 10;

        [SerializeField] [Tooltip("自动切片时间，值越大处理的请求数量越多，值越小处理请求的数量越小，可以根据目标帧率分配。")]
        private float maxUpdateTimeSlice = 1 / 60f;

        [SerializeField] [Tooltip("是否开启自动切片")] private bool autoSlicing = true;
    }
}