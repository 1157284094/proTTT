using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMode : UIBase
{
    public void OnBackClick()
    {
        GameRoot.Instance.uimgr.SwitchUI("UIMain");
    }

    public void OnNoAIClick()
    {

        GameRoot.Instance.uimgr.SwitchUI("UIGamePlay", () =>
        {
            GameRoot.Instance.StartGameNoAI();
        });
    }

    public void OnAIClick()
    {
        GameRoot.Instance.uimgr.SwitchUI("UIGameReady");
    }
}
