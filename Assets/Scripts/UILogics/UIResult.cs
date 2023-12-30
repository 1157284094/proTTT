using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIResult : UIBase
{
    public TextMeshProUGUI text = null;
    const string result1 = "O 获胜！";
    const string result2 = "X 获胜！";
    const string result3 = "平局";

    private void OnEnable()
    {
        var result = GameRoot.Instance.GetGameResult();
        switch (result)
        {
            case CheckResult.Player1Win:
                text.text = result1;
                break;
            case CheckResult.Player2Win:
                text.text = result2;
                break;
            case CheckResult.Draw:
                text.text = result3;
                break;
        }
    }

    public void OnRestartClick()
    {
        GameRoot.Instance.uimgr.SwitchUI("UIGamePlay", () =>
        {
            if (GameRoot.Instance.IsAIEnable())
            {
                GameRoot.Instance.StartGameWithAI();
            }
            else
            {
                GameRoot.Instance.StartGameNoAI();
            }
            //GameRoot.Instance.SwitchGamePlay(true);
        });
    }
}
