using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// �����̸���ʱ�������¼�
/// ��һ������Ϊx���꣬�ڶ�������Ϊy���꣬����������Ϊ��ұ��
/// </summary>
[System.Serializable]
public class MoveEvent : UnityEngine.Events.UnityEvent<int, int, int> { }

public class GamePlayData
{
    //��ʼ��Ϊ0��player1Ϊ1��player2Ϊ2
    private int[,] tiles = new int[3, 3];
    public int[,] Tiles
    {
        get
        {
            return tiles;
        }
    }

    //TODO��NO MAGIC NUMBER!!
    private int currentPlayer = 0;
    public int CurrentPlayerSymbol
    {
        get
        {
            return currentPlayer + 1;
        }
    }

    public MoveEvent onPlayerMoved = new MoveEvent();

    public void ResetData()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                tiles[i, j] = 0;
            }
        }
    }

    /// <summary>
    /// ����������ң�0Ϊplayer1��1Ϊplayer2
    /// </summary>
    /// <param name="player"></param>
    public void SetFirstMovePlayer(int player)
    {
        currentPlayer = player;
    }

    public void MakeMove(int x, int y)
    {
        if (tiles[x, y] == 0)
        {
            tiles[x, y] = currentPlayer + 1;
            //�������̸����¼�
            var movedPlayer = currentPlayer;
            currentPlayer = 1 - currentPlayer;
            onPlayerMoved.Invoke(x, y, movedPlayer);
        }
    }

    /// <summary>
    /// ��ȡ������ĳ��λ�õ���������
    /// </summary>
    public int GetTileType(int x, int y)
    {
        return tiles[x, y];
    }

    public bool CheckRowWin(int row)
    {
        return CheckTilesRowWin(tiles, row);
    }

    public bool CheckColumnWin(int column)
    {
        return CheckTilesColumnWin(tiles, column);
    }

    public bool CheckDiagonalWin(int row, int colum)
    {
        return CheckTilesDiagonalWin(tiles, row, colum);
    }

    public bool IsTileFull()
    {
        return IsTilesFull(tiles);
    }

    public CheckResult CheckWinCondition(int lastX, int lastY)
    {
        return CheckTilesWinCondition(tiles, lastX, lastY);
    }

    public static bool CheckTilesRowWin(int[,] tiles, int row)
    {
        return tiles[row, 0] == tiles[row, 1] && tiles[row, 1] == tiles[row, 2] && tiles[row, 0] != 0;
    }

    public static bool CheckTilesColumnWin(int[,] tiles, int column)
    {
        return tiles[0, column] == tiles[1, column] && tiles[1, column] == tiles[2, column] && tiles[0, column] != 0;
    }

    public static bool CheckTilesDiagonalWin(int[,] tiles, int row, int colum)
    {
        if (row != colum && row + colum != 2)
        {
            return false;
        }

        return tiles[0, 0] == tiles[1, 1] && tiles[1, 1] == tiles[2, 2] && tiles[0, 0] != 0 ||
            tiles[0, 2] == tiles[1, 1] && tiles[1, 1] == tiles[2, 0] && tiles[0, 2] != 0;
    }

    public static CheckResult CheckTilesWinCondition(int[,] tiles, int lastX, int lastY)
    {
        int playerToCheck = tiles[lastX, lastY];

        if (CheckTilesRowWin(tiles, lastX) || CheckTilesColumnWin(tiles, lastY) || CheckTilesDiagonalWin(tiles, lastX, lastY))
        {
            return (CheckResult)playerToCheck;
        }

        if (IsTilesFull(tiles))
        {
            return CheckResult.Draw;
        }

        return CheckResult.GoOn;
    }

    public static bool IsTilesFull(int[,] tiles)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (tiles[i, j] == 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static bool IsGameOver(int[,] tiles)
    {
        for (int i = 0; i < 3; i++)
        {
            if (CheckTilesRowWin(tiles, i))
            {
                return true;
            }
            if (CheckTilesColumnWin(tiles, i))
            {
                return true;
            }
            if (CheckTilesDiagonalWin(tiles, i, i))
            {
                return true;
            }
        }
        if (IsTilesFull(tiles))
        {
            return true;
        }
        return false;
    }

#if UNITY_EDITOR
    public void DebugGamePlay()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("\r\n");
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                sb.Append(tiles[i, j]);
            }
            sb.Append("\r\n");
        }
        Debug.Log(sb.ToString());
    }
#endif
}
