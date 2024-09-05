using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TutorialWindowPosition
{
    Top, Bottom
}

[System.Serializable]
public struct TutorialStage
{
    public string id, completeAction, actionForMessage;
    public float showTime;
    public TutorialWindowPosition windowPosition;

    [TextArea] public string message;

    public UnityEvent OnStageCompleted, OnStageStarted;
}

public class TutorialController : Singleton<TutorialController>
{
    public bool IsActive { get; private set; }

    [SerializeField] private TutorialStage[] stages;

    private int currentStageIndex = -1;
    private bool messageShowed = false;

    public void StartTutorial()
    {
        currentStageIndex = 0;

        IsActive = true;

        StartStage();
    }

    private TutorialStage GetCurrentStage()
    {
        return stages[currentStageIndex];
    }

    public bool IsActiveAndCurrentStageIs(string id)
    {
        if (!IsActive || currentStageIndex < 0)
        {
            return false;
        }

        return GetCurrentStage().id.Equals(id);
    }

    private void StartStage()
    {
        TutorialStage tutorialStage = GetCurrentStage();
        tutorialStage.OnStageStarted?.Invoke();

        messageShowed = false;

        if (tutorialStage.windowPosition == TutorialWindowPosition.Top)
        {
            GameController.Instance.UI.TutorialPanel.SetOnTop();

        }
        else if (tutorialStage.windowPosition == TutorialWindowPosition.Bottom)
        {
            GameController.Instance.UI.TutorialPanel.SetOnBottom();
        }

        if (string.IsNullOrWhiteSpace(tutorialStage.actionForMessage))
        {
            ShowMessagePanel();
        }

        if (string.IsNullOrWhiteSpace(tutorialStage.completeAction))
        {
            Invoke(nameof(NextStage), tutorialStage.showTime);
        }
    }

    private void ShowMessagePanel()
    {
        TutorialStage tutorialStage = GetCurrentStage();
        GameController.Instance.UI.TutorialPanel.Show(tutorialStage.message);

        messageShowed = true;
    }

    public void MakeAction(string value)
    {
        if (!IsActive || currentStageIndex < 0)
        {
            return;
        }

        TutorialStage tutorialStage = GetCurrentStage();
        if (!messageShowed)
        {
            if (value.Equals(tutorialStage.actionForMessage))
            {
                ShowMessagePanel();
            }

            return;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            if (!string.IsNullOrWhiteSpace(tutorialStage.completeAction))
            {
                return;
            }
        }

        if(!value.Equals(tutorialStage.completeAction)) 
        {
            return;
        }

     //   tutorialStage.OnStageCompleted?.Invoke();

        NextStage();
    }

    public void NextStage()
    {
        GameController.Instance.UI.TutorialPanel.Hide();

        currentStageIndex++;
        if (currentStageIndex >= stages.Length)
        {
            ScenesLoader.LoadMenu();

            return;
        }

        StartStage();
    }
}