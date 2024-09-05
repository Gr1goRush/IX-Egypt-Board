using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public MainPanel MainPanel => mainPanel;
    [SerializeField] private MainPanel mainPanel;
    public RollPanel RollPanel => rollPanel;
    [SerializeField] private RollPanel rollPanel;
    public TutorialPanel TutorialPanel => tutorialPanel;
    [SerializeField] private TutorialPanel tutorialPanel;
    public CompletePanel CompletePanel => completePanel;
    [SerializeField] private CompletePanel completePanel;
}
