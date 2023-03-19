using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PKFramework
{
    public abstract class BasePanel : BaseView
    {
        UILayer uiLayer = UILayer.CommonUI;
        public abstract void Open();

        public abstract void Close();

        public override void InitData(UIParamsBase uiParams)
        {
           
        }
    }

}
