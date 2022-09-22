using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PKFramework
{

    public abstract class BaseSubPanel : BaseView
    {
        private Transform parent;
        public override void InitData(UIParamsBase uiParams)
        {
            UIParamsWithParent panelParams = uiParams as UIParamsWithParent;
            if(panelParams == null)
            {
                PKLoger.LogError("SubPanel doesn't use UIParamsWithParent");
                return;
            }
        }
    }

}
