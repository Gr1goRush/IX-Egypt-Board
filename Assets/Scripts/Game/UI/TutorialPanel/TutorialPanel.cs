using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI messageText;

    [SerializeField] private float bottomY, topY;

    public void SetOnTop()
    {
        SetPosition(topY);
    }

    public void SetOnBottom()
    {
        SetPosition(bottomY);
    }

    private void SetPosition(float y)
    {
        rectTransform.anchoredPosition = new Vector2(0, y);
    }

    public void Show(string message)
    {
        gameObject.SetActive(true);
        messageText.text = message;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
