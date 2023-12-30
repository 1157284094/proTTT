using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGamePlay : UIBase
{
    public void OnQuitClick()
    {
        GameRoot.Instance.uimgr.SwitchUI("UIMain");
        GameRoot.Instance.QuitGame();
    }
}
