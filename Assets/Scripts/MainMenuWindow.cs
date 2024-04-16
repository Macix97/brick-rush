using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : UIWindow
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button quitButton;

    protected override void Awake()
    {
        base.Awake();
        startButton.onClick.AddListener(GameManager.LoadGame);
        leaderboardButton.onClick.AddListener(() => GameManager.SetState(GameState.Leaderboard));
        quitButton.onClick.AddListener(GameManager.QuitGame);
    }
}
