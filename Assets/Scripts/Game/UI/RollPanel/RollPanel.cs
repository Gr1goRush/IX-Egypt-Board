using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public delegate void GameAction();

public class RollPanel : MonoBehaviour
{
    [SerializeField] private GameObject defaultPanel, resultPanel;
    [SerializeField] private Button rollButton;
    [SerializeField] private SVGImage[] dices;
    [SerializeField] private Sprite[] dicesSprites;

    public event GameAction OnRollCompleted;

    public void Show(int result, bool waitClickToRoll)
    {
        List<int> dicesValues = Enumerable.Range(1, 5).ToList();
        dicesValues.Remove(result);
        dicesValues = Utility.GetRandomEnumerable(dicesValues).ToList();
        dicesValues.Insert(0, result);

        for (int i = 0; i < dices.Length; i++)
        {
            dices[i].sprite = dicesSprites[dicesValues[i] - 1];
        }

        defaultPanel.SetActive(true);
        resultPanel.SetActive(false);
        gameObject.SetActive(true);

        if (waitClickToRoll)
        {
            rollButton.interactable = true;
        }
        else
        {
            rollButton.interactable = false;
            Invoke(nameof(Roll), 1.5f);
        }
    }

    public void Roll()
    {
        AudioController.Instance.Sounds.PlayOneShot("roll");
        TutorialController.Instance.MakeAction("roll");

        defaultPanel.SetActive(false);
        resultPanel.SetActive(true);

        Invoke(nameof(CompleteRoll), 1.5f);
    }

    private void CompleteRoll()
    {
        gameObject.SetActive(false);

        OnRollCompleted?.Invoke();
    }
}
