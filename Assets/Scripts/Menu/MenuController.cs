using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel, multiplayerPanel, settingsPanel;

    private void Start()
    {
        mainPanel.SetActive(true);    
    }

    public void PlaySingleplayer()
    {
        GameModeManager.Instance.SelectSingleplayer(false);
        ScenesLoader.LoadGame();
    }

    public void HowToPlay()
    {
        GameModeManager.Instance.SelectSingleplayer(true);
        ScenesLoader.LoadGame();
    }

    public void ShowMultiplayerPanel()
    {
        mainPanel.SetActive(false);
        multiplayerPanel.SetActive(true);
    }

    public void HideMultiplayerPanel()
    {
        mainPanel.SetActive(true);
        multiplayerPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void HideSettings()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }
}
