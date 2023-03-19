using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PKFramework
{
    public class UIParamsBase
    {

    }

    public class UIParamsWithParent : UIParamsBase
    {
        public Transform paranet;
    }

    public abstract class BaseView
    {
        int m_id;

        int m_uniqId;

        string m_path;

        public abstract void Load();

        public abstract void UnLoad();

        public abstract void InitData(UIParamsBase uiParams);

        public abstract void InitUI();
    }
}

