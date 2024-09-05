using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanel : MonoBehaviour
{
    [SerializeField] private GamePlayerPanel[] playersPanels;
    [SerializeField] private DancerHousePanel[] housesPanels;

    private void Start()
    {
        int playersCount = GameModeManager.Instance.PlayersCount;
        for (int i = 0; i < playersPanels.Length; i++)
        {
            GamePlayerPanel gamePlayerPanel = playersPanels[i];
            if(i >= playersCount)
            {
                gamePlayerPanel.gameObject.SetActive(false);
                continue;
            }

            gamePlayerPanel.gameObject.SetActive(true);

            PlayerData playerData = PlayersManager.Instance.GetPlayerData(i);
            gamePlayerPanel.SetName(playerData.name);

            PlayerVisual playerVisual = PlayersManager.Instance.GetVisual(playerData.visualIndex);
            gamePlayerPanel.SetVisual(playerVisual);
        }
    }

    public void SetDancersCount(int playerIndex, int dancersCount)
    {
        playersPanels[playerIndex].SetDancersCount(dancersCount);
    }

    public void SetHouses(Dictionary<BoardHouse, int> housesPlayersDictionary)
    {
        for (int i = 0; i < housesPanels.Length; i++)
        {
            DancerHousePanel dancerHousePanel = housesPanels[i];
            BoardHouse house = dancerHousePanel.House;
            if(!housesPlayersDictionary.ContainsKey(house) || housesPlayersDictionary[house] < 0)
            {
                dancerHousePanel.SetEmpty();
            }
            else
            {
                PlayerVisual playerVisual = PlayersManager.Instance.GetPlayerVisual(housesPlayersDictionary[house]);
                dancerHousePanel.SetPlayerDancer(playerVisual.dancerIcon);

            }
        }
    }
}
