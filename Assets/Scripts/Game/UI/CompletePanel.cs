using System.Collections;
using TMPro;
using UnityEngine;

public class CompletePanel : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI nameText, timeText;

    public void Show(string playerName, int minutes)
    {
        nameText.text = playerName;
        timeText.text = "In " + minutes.ToString() + " Minutes!";
        gameObject.SetActive(true);
    }

    public void Menu()
    {
        ScenesLoader.LoadMenu();
    }
}