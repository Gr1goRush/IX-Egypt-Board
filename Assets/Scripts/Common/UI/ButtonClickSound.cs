using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(ClickButton);
    }

    void ClickButton()
    {
        AudioController.Instance.Sounds.PlayOneShot("button_click");
    }
}
