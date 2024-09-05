using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MultiplayerPlayerOptionsPanel : MonoBehaviour
{
    public int SelectedVisualIndex { get; private set; }

    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private SVGImage nameFieldImage;
    [SerializeField] private Image portraitBorderImage, portraitImage;
    [SerializeField] private Button prevButton, nextButton;

    public void SetVisual(int index, MultiplayerPanelPlayerVisual multiplayerPanelVisual, PlayerVisual playerVisual)
    {
        SelectedVisualIndex = index;

        SetSmallButtonSprite(prevButton, multiplayerPanelVisual.smallButtonSprite);
        SetSmallButtonSprite(nextButton, multiplayerPanelVisual.smallButtonSprite);

        nameFieldImage.sprite = multiplayerPanelVisual.inputFieldSprite;

        portraitBorderImage.sprite = playerVisual.border;
        portraitImage.sprite = playerVisual.icon;
    }

    private void SetSmallButtonSprite(Button button, Sprite sprite)
    {
        Image _image = button.targetGraphic as Image;
        _image.sprite = sprite;
    }

    public void SetButtonsInteractable(bool prev, bool next)
    {
        prevButton.interactable = prev;
        nextButton.interactable = next;
    }

    public void AddPrevButtonClickListener(UnityAction action)
    {
        prevButton.onClick.AddListener(action);
    }

    public void AddNextButtonClickListener(UnityAction action)
    {
        nextButton.onClick.AddListener(action);
    }

    public PlayerData GetPlayerData()
    {
        return new PlayerData { name = nameField.text, visualIndex = SelectedVisualIndex };
    }
}
