using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseWindow : UIWindow
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button exitButton;

    protected override void Awake()
    {
        base.Awake();
        resumeButton.onClick.AddListener(GameManager.SetPreviousState);
        exitButton.onClick.AddListener(GameManager.LoadMainMenu);
    }
}
