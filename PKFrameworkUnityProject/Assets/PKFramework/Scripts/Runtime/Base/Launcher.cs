using System;
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

        private void InitFrameworkComponents()
        {
            
        }
    }
}