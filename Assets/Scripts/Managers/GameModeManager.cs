using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
    Singleplayer, Multiplayer
}

public class GameModeManager : Singleton<GameModeManager>
{
    public GameMode GameMode {get;private set;}
     public int PlayersCount { get; private set; }
    public bool Tutorial { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        GameMode = GameMode.Singleplayer;
        PlayersCount = 2;
    }

    public void SelectSingleplayer(bool tutorial)
    {
        GameMode = GameMode.Singleplayer;
        PlayersCount = 2;
        Tutorial = tutorial;
        PlayersManager.Instance.SetRandomPlayers(2);
    }

    public void SelectMutliplayer(int playersCount)
    {
        GameMode = GameMode.Multiplayer;
        PlayersCount = playersCount;
        Tutorial = false;
    }
}
