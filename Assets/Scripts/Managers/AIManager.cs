using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIManager
{
    private int aiSymbol;
    public int AISymbol
    {
        get
        {
            return aiSymbol;
        }
    }

    private float smartMoveProbability;
    private int intelligenceLevel;

    private int[,] tiles
    {
        get
        {
            return GameRoot.Instance.GetGameDataTiles();
        }
    }

    public AIManager(int symbil, float smartMoveProbability, int intelligenceLevel)
    {
        this.aiSymbol = symbil;
        this.smartMoveProbability = smartMoveProbability;
        this.intelligenceLevel = intelligenceLevel;
    }

    public (int, int) GetNextMove()
    {
        //第一步随机选位置
        if (IsFirstStep())
        {
            return GetRandomMove();
        }

        if (Random.Range(0f, 1f) < smartMoveProbability)
        {
            return GetSmartMove();
        }
        else
        {
            return GetRandomMove();
        }
    }

    private bool IsFirstStep()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (tiles[i, j] != 0)
                    return false;
            }
        }
        return true;
    }

    private (int, int) GetRandomMove()
    {
        var emptyCells = new List<(int, int)>();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (tiles[i, j] == 0)
                {
                    emptyCells.Add((i, j));
                }
            }
        }

        if (emptyCells.Count == 0)
        {
            throw new System.InvalidOperationException("No empty cell!");
        }

        var move = emptyCells[Random.Range(0, emptyCells.Count)];
        return move;
    }

    private (int, int) GetSmartMove()
    {
        var isMaximizing = aiSymbol == 1;

        return FindBestMove(isMaximizing);

    }

    public (int, int) FindBestMove(bool isMaximizing)
    {
        //拷贝以避免修改原数组，因为3x3足够简单所以这个拷贝不会成为瓶颈
        var localTiles = (int[,])tiles.Clone();

        int bestScore = isMaximizing ? int.MinValue : int.MaxValue;
        List<(int, int)> bestMoves = new List<(int, int)>();

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (localTiles[i, j] == 0)
                {
                    localTiles[i, j] = isMaximizing ? 1 : 2;
                    int score = MiniMax(localTiles, 0, !isMaximizing);
                    localTiles[i, j] = 0;

                    if (isMaximizing && score > bestScore)
                    {
                        bestScore = score;
                        bestMoves.Clear();
                        bestMoves.Add((i, j));
                    }
                    else if (!isMaximizing && score < bestScore)
                    {
                        bestScore = score;
                        bestMoves.Clear();
                        bestMoves.Add((i, j));
                    }
                    else if (score == bestScore)
                    {
                        bestMoves.Add((i, j));
                    }
                }
            }
        }

        var bestMove = bestMoves[Random.Range(0, bestMoves.Count)];
        return bestMove;
    }

    private int MiniMax(int[,] tiles, int depth, bool isMaximizing)
    {
        int playerToCheck = isMaximizing ? 1 : 2;

        if (GamePlayData.IsGameOver(tiles) || depth >= intelligenceLevel)
        {
            int score = EvaluateTilesForPlayer(tiles, playerToCheck);
            return isMaximizing ? score : -score;
        }

        int bestScore = isMaximizing ? int.MinValue : int.MaxValue;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (tiles[i, j] == 0)
                {
                    tiles[i, j] = isMaximizing ? 1 : 2;
                    int score = MiniMax(tiles, depth + 1, !isMaximizing);
                    tiles[i, j] = 0;

                    if (isMaximizing && score > bestScore)
                    {
                        bestScore = score;
                    }
                    else
                    {
                        bestScore = score;
                    }
                }
            }
        }

        return bestScore;
    }

    /// <summary>
    /// 评估当前步的分数
    /// </summary>
    /// <returns></returns>
    private int EvaluateTilesForPlayer(int[,] tiles, int playerSymbol)
    {
        int score = 0;

        // 检查行和列
        for (int i = 0; i < 3; i++)
        {
            score += EvaluateLine(tiles[i, 0], tiles[i, 1], tiles[i, 2], playerSymbol);
            score += EvaluateLine(tiles[0, i], tiles[1, i], tiles[2, i], playerSymbol);
        }

        // 检查对角线
        score += EvaluateLine(tiles[0, 0], tiles[1, 1], tiles[2, 2], playerSymbol);
        score += EvaluateLine(tiles[0, 2], tiles[1, 1], tiles[2, 0], playerSymbol);

        return score;
    }

    private int EvaluateLine(int cell1, int cell2, int cell3, int playerSymbol)
    {
        int score = 0;
        int opponentSymbol = playerSymbol == 1 ? 2 : 1;

        int[] line = new int[] { cell1, cell2, cell3 };
        int playerCount = line.Count(c => c == playerSymbol);
        int opponentCount = line.Count(c => c == opponentSymbol);

        if (playerCount == 3)
            score = 10;
        else if (playerCount == 2 && line.Contains(0))
            score = 5;
        else if (playerCount == 1 && line.Count(c => c == 0) == 2)
            score = 1;

        if (opponentCount == 2 && line.Contains(0))
            score -= 5;

        return score;
    }
}
