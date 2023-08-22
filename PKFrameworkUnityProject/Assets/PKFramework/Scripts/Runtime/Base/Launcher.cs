using System;
using PKFramework.Core;
using PKFramework.Runtime.Singleton;
using UnityEngine;

namespace PKFramework.Runtime
{
    /// <summary>
    /// 启动器
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("PK Framework/Launcher")]
    public class Launcher: MonoSingleton<Launcher>
    {

        private void Awake()
        {
            
        }

        private void Update()
        {
            PKFrameworkCore.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }
    }
}