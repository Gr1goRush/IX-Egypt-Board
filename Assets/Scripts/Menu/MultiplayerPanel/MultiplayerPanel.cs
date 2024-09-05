using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct MultiplayerPanelPlayerVisual
{
    public Sprite smallButtonSprite, inputFieldSprite;
}

public class MultiplayerPanel : MonoBehaviour
{
    [SerializeField] private Button decreasePlayersCountButton, increasePlayersCountButton;
    [SerializeField] private TextMeshProUGUI playersCountText;

    [SerializeField] private MultiplayerPlayerOptionsPanel[] playersPanels;
    [SerializeField] private MultiplayerPanelPlayerVisual[] visuals;

    private int playersCount = 2;

    void Start()
    {
        decreasePlayersCountButton.onClick.AddListener(DecreasePlayersCount);
        increasePlayersCountButton.onClick.AddListener(IncreasePlayersCount);

        SetPlayersCount(2);

        for (int i = 0; i < playersPanels.Length; i++)
        {
            SetPlayerVisual(i, i);

            int playerIndex = i;
            playersPanels[i].AddNextButtonClickListener(() => NextVisual(playerIndex));
            playersPanels[i].AddPrevButtonClickListener(() => PrevVisual(playerIndex));
        }
    }

    private void SetPlayersCount(int count)
    {
        playersCount = count;
        playersCountText.text = count.ToString() + " Players";

        for (int i = 0; i < playersPanels.Length; i++)
        {
            playersPanels[i].gameObject.SetActive(i < playersCount);
        }

        decreasePlayersCountButton.interactable = playersCount > PlayersManager.minPlayersCount;
        increasePlayersCountButton.interactable = playersCount < playersPanels.Length;
    }

    private void IncreasePlayersCount()
    {
        SetPlayersCount(playersCount + 1);
    }

    private void DecreasePlayersCount()
    {
        SetPlayersCount(playersCount - 1);
    }

    private void NextVisual(int playerIndex)
    {
        SetPlayerVisual(playerIndex, playersPanels[playerIndex].SelectedVisualIndex + 1);
    }

    private void PrevVisual(int playerIndex)
    {
        SetPlayerVisual(playerIndex, playersPanels[playerIndex].SelectedVisualIndex - 1);
    }

    private void SetPlayerVisual(int playerIndex, int visualIndex)
    {
        playersPanels[playerIndex].SetVisual(visualIndex, visuals[visualIndex], PlayersManager.Instance.GetVisual(visualIndex));
        playersPanels[playerIndex].SetButtonsInteractable(visualIndex > 0, visualIndex < (visuals.Length - 1));
    }

    public void Play()
    {
        PlayersManager.Instance.SetPlayersCount(playersCount);

        for (int i = 0; i < playersCount; i++)
        {
            PlayerData playerData = playersPanels[i].GetPlayerData();
            if(string.IsNullOrWhiteSpace(playerData.name))
            {
                return;
            }

            PlayersManager.Instance.SetPlayerData(i, playerData);
        }

        GameModeManager.Instance.SelectMutliplayer(playersCount);
        ScenesLoader.LoadGame();
    }
}
