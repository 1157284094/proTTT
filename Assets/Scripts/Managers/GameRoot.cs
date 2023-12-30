using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameRoot : MonoBehaviour
{
    private static GameRoot _instance = null;

    public static GameRoot Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameRoot>();
            }
            return _instance;
        }
    }

    public UIManager uimgr = null;
    public GameObject gamePlay = null;
    private GameObject gamePlayInstance = null;
    private GameController gameplayController = null;
    public AIConfig aiconfig = null;
    [HideInInspector]
    public int AIHard = 0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        uimgr.SwitchUI("UIMain");
    }

    public void StartGameNoAI()
    {
        if (gamePlayInstance == null)
        {
            gamePlayInstance = Instantiate(gamePlay);
            gameplayController = gamePlayInstance.GetComponent<GameController>();
            gameplayController.Init();
            gameplayController.SetGameFinishedCallback(OnGameResult);
            gameplayController.StartGame();
        }
        else
        {
            gameplayController.RestartGame();
        }
    }

    public void StartGameWithAI()
    {
        if (gamePlayInstance == null)
        {
            gamePlayInstance = Instantiate(gamePlay);
            gameplayController = gamePlayInstance.GetComponent<GameController>();
            gameplayController.Init();
            gameplayController.SetGameFinishedCallback(OnGameResult);
            gameplayController.StartGameWithAI(AIHard);
        }
        else
        {
            gameplayController.RestartGameWithAI(AIHard);
        }
    }

    public void SwitchGamePlay(bool isShow)
    {
        if (gamePlayInstance == null)
        {
            return;
        }
        gamePlayInstance.SetActive(isShow);
    }

    public void QuitGame()
    {
        if (gamePlayInstance != null)
        {
            Destroy(gamePlayInstance);
        }
    }

    public CheckResult GetGameResult()
    {
        return gameplayController.currentState;
    }

    public int[,] GetGameDataTiles()
    {
        return gameplayController.GetTilesData();
    }

    public bool IsAIEnable()
    {
        return gameplayController.ai != null;
    }

    public bool IsAIAction()
    {
        return gameplayController.ai != null && gameplayController.ai.AISymbol == gameplayController.gamePlayData.CurrentPlayerSymbol;
    }

    private void OnGameResult()
    {
        //SwitchGamePlay(false);
        uimgr.SwitchUI("UIResult");
    }
}
