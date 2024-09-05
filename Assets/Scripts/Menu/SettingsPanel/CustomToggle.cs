using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.VectorGraphics;

public class CustomToggle : MonoBehaviour
{
    [SerializeField] private SVGImage onImage, offImage;
    [SerializeField] private Sprite onSprite, offSprite, emptySprite;
    [SerializeField] private Button onButton, offButton;

    public bool IsOn
    {
        get { return isOn; }
        set { isOn = value; SetValue(isOn); }
    }
    private bool isOn;

    public UnityEvent<bool> onSwitched;

    private void Start()
    {
        onButton.onClick.AddListener(SetOn);
        offButton.onClick.AddListener(SetOff);
    }

    void SetValue(bool v)
    {
        onButton.interactable = !v;
        onImage.sprite = v ? onSprite : emptySprite;

        offButton.interactable = v;
        offImage.sprite = !v ? offSprite : emptySprite;
    }

    private void SetOff()
    {
        IsOn = false;
        onSwitched?.Invoke(isOn);
    }

    private void SetOn()
    {
        IsOn = true;
        onSwitched?.Invoke(isOn);
    }
}
