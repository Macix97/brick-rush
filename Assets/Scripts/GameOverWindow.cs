using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : UIWindow
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    protected override void Awake()
    {
        base.Awake();
        restartButton.onClick.AddListener(GameManager.LoadGame);
        mainMenuButton.onClick.AddListener(GameManager.LoadMainMenu);
    }
}
