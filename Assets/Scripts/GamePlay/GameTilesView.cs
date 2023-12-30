using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTilesView : MonoBehaviour
{
    public GameObject tilesRoot = null;

    public GameObject bgPrefab = null;
    public GameObject tilePrefab = null;

    private Transform tilesRootTransform = null;

    private TileView[,] tileViews = new TileView[3, 3];

    private float bgWidth = 0;
    private float bgHeight = 0;

    private float tileWidth = 0;
    private float tileHeight = 0;

    private void Awake()
    {
        if (tilesRoot == null)
        {
            tilesRootTransform = transform.Find("Tiles");
            tilesRoot = tilesRootTransform.gameObject;
        }
        else
        {
            tilesRootTransform = tilesRoot.transform;
        }
        //依据bgPrefab的Sprite计算boardSize
        ToolFunctions.GetTextureSize(bgPrefab.GetComponent<SpriteRenderer>(), out bgWidth, out bgHeight);
        tileWidth = bgWidth / 3;
        tileHeight = bgHeight / 3;
    }

    public void Init()
    {
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        float offsetX = bgWidth / 2 - tileWidth / 2;
        float offsetY = bgHeight / 2 - tileHeight / 2;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                //计算坐标
                var position = new Vector2(j * tileWidth - offsetX, i * tileHeight - offsetY);

                var tileInstance = Instantiate(tilePrefab, position, Quaternion.identity, tilesRootTransform);
                tileInstance.name = $"Tile_{i}_{j}";
                tileViews[i, j] = tileInstance.GetComponent<TileView>();
                tileViews[i, j].Init();
                tileViews[i, j].SetBoardPosition(i, j);
            }
        }
    }

    /// <summary>
    /// 依据棋盘数据更新显示
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="player"></param>
    public void UpdateTile(int x, int y)
    {
        tileViews[x, y].UpdateView();
    }

    public void UpdateAllTiles()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                tileViews[i, j].UpdateView();
            }
        }
    }
}
