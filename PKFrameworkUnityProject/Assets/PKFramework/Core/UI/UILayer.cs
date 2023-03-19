using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PKFramework
{
    public enum UILayer
    {
        //场景UI
        Scene = 0,
        //UI背景
        BackGround,
        //常用UI界面
        CommonUI,
        //顶层UI界面，比如新手引导界面
        TopUI,
        //游戏消息弹出，比如小tips，跑马灯
        Toast,
        //应用层消息，版本号，错误提示等
        System,
    }
}
