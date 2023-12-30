using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameReady : UIBase
{
    public Slider hardSlider = null;

    private void Awake()
    {
        hardSlider.maxValue = GameRoot.Instance.aiconfig.aiConfigs.Count;
    }

    public void OnBackClick()
    {
        GameRoot.Instance.uimgr.SwitchUI("UIMode");
    }

    public void OnStartGameClick()
    {
        int hard = (int)hardSlider.value - 1;
        GameRoot.Instance.uimgr.SwitchUI("UIGamePlay", () =>
        {
            GameRoot.Instance.AIHard = hard;
            GameRoot.Instance.StartGameWithAI();
        });
    }
}
