using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileView : MonoBehaviour
{
    public Sprite[] sprites = null;

    private GamePlayData gamePlayData = null;
    private int m_x = 0;
    private int m_y = 0;
    private SpriteRenderer spriteRenderer = null;

    // Start is called before the first frame update
    public void Init()
    {
        var controller = FindObjectOfType<GameController>();
        gamePlayData = controller.gamePlayData;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    public void SetBoardPosition(int x, int y)
    {
        m_x = x;
        m_y = y;
    }

    private void OnMouseDown()
    {
        //游戏结算时不能点击
        if (GameRoot.Instance.GetGameResult() != CheckResult.GoOn)
        {
            return;
        }
        //AI正在行动时不能点击
        if (GameRoot.Instance.IsAIAction())
        {
            return;
        }

        gamePlayData.MakeMove(m_x, m_y);

    }

    public void UpdateView()
    {
        var tileType = gamePlayData.GetTileType(m_x, m_y);
        if (tileType == 0)
        {
            spriteRenderer.enabled = false;
        }
        else
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = sprites[tileType - 1];
        }
    }
}
