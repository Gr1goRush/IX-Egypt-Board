using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct PlayerData
{
    public string name;
    public int visualIndex;
}

[System.Serializable]
public struct PlayerVisual
{
    public Sprite border, icon, dancerIcon;
}

public class PlayersManager : Singleton<PlayersManager>
{
    public int PlayersCount => players.Length;

    [SerializeField] private PlayerVisual[] visuals;

    private PlayerData[] players;

    public const int minPlayersCount = 2;

    protected override void Awake()
    {
        base.Awake();

        if(players == null || players.Length == 0)
        {
            SetRandomPlayers(2);
        }
    }

    public void SetRandomPlayers(int count)
    {
        int[] visualsIndexes = Utility.GetRandomEnumerable(Enumerable.Range(0, visuals.Length)).ToArray();
        SetPlayersCount(count);

        for (int i = 0; i < count; i++)
        {
            SetPlayerData(i, new PlayerData
            {
                name = "Player " + (i + 1).ToString(),
                visualIndex = visualsIndexes[i],
            });
        }
    }

    public PlayerData GetPlayerData(int playerIndex)
    {
        return players[playerIndex];
    }

    public void SetPlayerData(int playerIndex, PlayerData playerData)
    {
        players[playerIndex] = playerData;
    }

    public void SetPlayersCount(int count)
    {
        players = new PlayerData[count];
    }

    public PlayerVisual GetVisual(int index)
    {
        return visuals[index];
    }

    public PlayerVisual GetPlayerVisual(int playerIndex)
    {
        return visuals[GetPlayerData(playerIndex).visualIndex];
    }

    public RuntimeAnimatorController LoadDancerAnimatorController(int playerIndex)
    {
        int index = GetPlayerData(playerIndex).visualIndex;
        return Resources.Load<RuntimeAnimatorController>("Dancers/Dancer" +  index + "AnimatorController");
    }

    public int GetVisualsCount()
    {
        return visuals.Length;
    }
}
