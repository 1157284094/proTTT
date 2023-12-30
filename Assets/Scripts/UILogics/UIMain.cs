using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMain : UIBase
{
    public void OnStartClick()
    {
        GameRoot.Instance.uimgr.SwitchUI("UIMode");
    }

    public void OnSettingsClick()
    {
        //GameRoot.Instance.uimgr.SwitchUI("");
    }

    public void OnQuitGameClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
