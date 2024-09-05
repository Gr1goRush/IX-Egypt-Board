using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;

public class DancerHousePanel : MonoBehaviour
{
    public BoardHouse House => house;
    [SerializeField] private BoardHouse house;

    [SerializeField] private SVGImage dancerImage;

    public void SetEmpty()
    {
        dancerImage.enabled = false;
    }

    public void SetPlayerDancer(Sprite dancerSprite)
    {
        dancerImage.sprite = dancerSprite;
        dancerImage.enabled = true;
    }
}
