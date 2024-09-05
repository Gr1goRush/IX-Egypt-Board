using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void GameActionInt(int v);

public class BoardPlayer
{
    public readonly bool isAI;

    public BoardPlayer(bool isAI)
    {
        this.isAI = isAI;
    }
}

public class GameController : Singleton<GameController>
{
    [SerializeField] private Board board;

    public GameUI UI => gameUI;
    [SerializeField] private GameUI gameUI;

    private int movingPlayerIndex = -1, rollResult = -1;
    private GameActionInt onRoll;

    private int gameTime = 0;
    private bool gameIsActive = false;

    private BoardPlayer[] players;

    private void Start()
    {
        int playersCount = GameModeManager.Instance.PlayersCount;
        board.Initialize(playersCount);

        players = new BoardPlayer[playersCount];
        bool withAI = GameModeManager.Instance.GameMode == GameMode.Singleplayer;
        for (int i = 0; i < playersCount; i++)
        {
            BoardPlayer boardPlayer = new BoardPlayer(withAI && i != 0);
            players[i] = boardPlayer;

            UpdateDancersCount(i);
        }

        UI.RollPanel.OnRollCompleted += OnRollCompleted;

        movingPlayerIndex = 0;
        gameTime = 0;

        gameIsActive = true;
        StartCoroutine(Timing());

        UpdateHousesUI();

        Vector3 boardPos = board.GetBoardPosition();
        Vector3 boardSize = board.GetBoardSize();
        CameraController.Instance.SetSize(boardPos, boardSize);

        Invoke(nameof(StartGame), 1f);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ScenesLoader.LoadMenu();
        }
    }

    IEnumerator Timing()
    {
        while (gameIsActive)
        {
            yield return new WaitForSeconds(1);
            gameTime++;
        }
    }

    private void StartGame()
    {
        if (GameModeManager.Instance.Tutorial)
        {
            TutorialController.Instance.StartTutorial();
        }

        StartMove();
    }

    private void StartMove()
    {
        Roll(OnMoveRollCompleted);
    }

    public void Roll(GameActionInt _onRoll)
    {
        onRoll = _onRoll;

        if (TutorialController.Instance.IsActive)
        {
            rollResult = 2;
        }
        else
        {
            rollResult = Random.Range(1, 6);
        }

        UI.RollPanel.Show(rollResult, !MovingPlayerIsAI());
    }

    private void OnRollCompleted()
    {
        onRoll?.Invoke(rollResult);
    }

    private bool MovingPlayerIsAI()
    {
        return players[movingPlayerIndex].isAI;
    }

    private void OnMoveRollCompleted(int result)
    {
        rollResult = result;
        TutorialController.Instance.MakeAction("roll_completed");

        if (TutorialController.Instance.IsActive && MovingPlayerIsAI())
        {
            board.HighlightOnlyOneDancer(movingPlayerIndex, 1);
            Invoke(nameof(ProcessPlayerMove), 1.5f);
        }
        else
        {
            ProcessPlayerMove();
        }
    }

    private void ProcessPlayerMove()
    {
        board.ProcessPlayerMove(movingPlayerIndex, rollResult, MovingPlayerIsAI());
    }

    private void UpdateHousesUI()
    {
        Dictionary<BoardHouse, int> housesPlayersDictionary = board.GetHousesPlayers();
        UI.MainPanel.SetHouses(housesPlayersDictionary);
    }

    public void OnMoveCompleted()
    {
        TutorialController.Instance.MakeAction("complete_move");

        UpdateHousesUI();

        int dancersCount = board.GetActiveDancersCount(movingPlayerIndex);
        UpdateDancersCount(movingPlayerIndex, dancersCount);

        if (dancersCount <= 0)
        {
            CompleteGame();
            return;
        }

        movingPlayerIndex++;
        if (movingPlayerIndex >= players.Length)
        {
            movingPlayerIndex = 0;

            if (TutorialController.Instance.IsActive)
            {
                board.HighlightOnlyOneDancer(1, 1);

                return;
            }
        }

        StartMove();
    }

    private void CompleteGame()
    {
        gameIsActive = false;
        board.ShowDestroyAnimation(OnBoardDestroyAnimationCompleted);
    }

    private void OnBoardDestroyAnimationCompleted()
    {
        string playerName = PlayersManager.Instance.GetPlayerData(movingPlayerIndex).name;
        int gameMinutes = Mathf.CeilToInt(gameTime / 60f);
        UI.CompletePanel.Show(playerName, gameMinutes);
    }

    private void UpdateDancersCount(int playerIndex)
    {
        UpdateDancersCount(playerIndex, board.GetActiveDancersCount(playerIndex));
    }

    private void UpdateDancersCount(int playerIndex, int count)
    {
        UI.MainPanel.SetDancersCount(playerIndex, count);
    }
}
