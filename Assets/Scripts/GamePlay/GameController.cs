using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

public enum CheckResult
{
    GoOn,
    Player1Win,
    Player2Win,
    Draw,
}


public class GameController : MonoBehaviour
{
    public GamePlayData gamePlayData = null;
    public GameTilesView gameTilesView = null;

    [HideInInspector]
    public AIManager ai = null;
    /// <summary>
    /// 游戏结束时回调，只由GameRoot监听
    /// </summary>
    private UnityAction onGameFinished = null;

    public CheckResult currentState = CheckResult.Draw;

    private void Awake()
    {
        gamePlayData = new GamePlayData();
        if (gameTilesView == null)
        {
            gameTilesView = GetComponent<GameTilesView>();
        }
    }

    public void Init()
    {
        gameTilesView.Init();
        gamePlayData.onPlayerMoved.AddListener(HandleMove);
    }

    public void StartGame()
    {
        ai = null;
        currentState = CheckResult.GoOn;
        //默认每次都从player1开始
        gamePlayData.SetFirstMovePlayer(0);
    }

    public void StartGameWithAI(int AILevel)
    {
        var aiconfig = GameRoot.Instance.aiconfig;
        if (aiconfig == null || aiconfig.aiConfigs.Count <= AILevel)
        {
            throw new System.ArgumentOutOfRangeException("AILevel", "AILevel should be less than the number of AI configs");
        }

        currentState = CheckResult.GoOn;
        //默认每次都从player1开始
        gamePlayData.SetFirstMovePlayer(0);

        var aicfg = aiconfig.aiConfigs[AILevel];
        //随机1或者2
        var aiPlayerSymbol = Random.Range(1, 3);
        ai = new AIManager(aiPlayerSymbol, aicfg.smartMoveProbability, aicfg.intelligenceLevel);
        if (aiPlayerSymbol == 1)
        {
            var (x, y) = ai.GetNextMove();
            gamePlayData.MakeMove(x, y);
        }
    }

    /// <summary>
    /// 注册游戏结束时的回调
    /// </summary>
    /// <param name="callback"></param>
    public void SetGameFinishedCallback(UnityAction callback)
    {
        if (onGameFinished != null)
        {
            return;
        }

        onGameFinished = callback;
    }

    public void RestartGame()
    {
        gamePlayData.ResetData();
        gameTilesView.UpdateAllTiles();

        StartGame();
    }

    public void RestartGameWithAI(int AIHard)
    {
        gamePlayData.ResetData();
        gameTilesView.UpdateAllTiles();

        StartGameWithAI(AIHard);
    }

    void HandleMove(int x, int y, int player)
    {
        gameTilesView.UpdateTile(x, y);

        var oldState = currentState;
        currentState = CheckWinCondition(x, y);
        if (oldState == CheckResult.GoOn && currentState != CheckResult.GoOn)
        {
            onGameFinished?.Invoke();
        }
        else
        {
            if (ai != null && gamePlayData.CurrentPlayerSymbol == ai.AISymbol)
            {
                var (i, j) = ai.GetNextMove();
                gamePlayData.MakeMove(i, j);
            }
        }
    }

    CheckResult CheckWinCondition(int lastX, int lastY)
    {
        //#if UNITY_EDITOR
        //        gamePlayData.DebugGamePlay();
        //#endif

        return gamePlayData.CheckWinCondition(lastX, lastY);
    }

    public int[,] GetTilesData()
    {
        return gamePlayData.Tiles;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GameController))]
    public class GameControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("打印棋盘数据 "))
            {
                var script = target as GameController;
                script.gamePlayData.DebugGamePlay();
            }
        }
    }
#endif
}
