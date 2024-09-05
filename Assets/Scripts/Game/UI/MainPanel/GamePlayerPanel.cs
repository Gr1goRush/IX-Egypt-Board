using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayerPanel : MonoBehaviour
{
    [SerializeField] private Image borderImage, iconImage;
    [SerializeField] private SVGImage dancerImage;
    [SerializeField] private TextMeshProUGUI nameText, dancersCountText;

    public void SetName(string playerName)
    {
        nameText.text = playerName;
    }

    public void SetVisual(PlayerVisual playerPortrait)
    {
        borderImage.sprite = playerPortrait.border;
        iconImage.sprite = playerPortrait.icon;
        dancerImage.sprite = playerPortrait.dancerIcon;
    }

    public void SetDancersCount(int count)
    {
        dancersCountText.text = count.ToString();
    }
}
