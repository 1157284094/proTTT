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
    /// ��Ϸ����ʱ�ص���ֻ��GameRoot����
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
        //Ĭ��ÿ�ζ���player1��ʼ
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
        //Ĭ��ÿ�ζ���player1��ʼ
        gamePlayData.SetFirstMovePlayer(0);

        var aicfg = aiconfig.aiConfigs[AILevel];
        //���1����2
        var aiPlayerSymbol = Random.Range(1, 3);
        ai = new AIManager(aiPlayerSymbol, aicfg.smartMoveProbability, aicfg.intelligenceLevel);
        if (aiPlayerSymbol == 1)
        {
            var (x, y) = ai.GetNextMove();
            gamePlayData.MakeMove(x, y);
        }
    }

    /// <summary>
    /// ע����Ϸ����ʱ�Ļص�
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
            if (GUILayout.Button("��ӡ�������� "))
            {
                var script = target as GameController;
                script.gamePlayData.DebugGamePlay();
            }
        }
    }
#endif
}
