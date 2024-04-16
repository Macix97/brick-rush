using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverWindow : UIWindow
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI recordText;

    protected override void Awake()
    {
        base.Awake();
        recordText.enabled = false;
        restartButton.onClick.AddListener(GameManager.LoadGame);
        mainMenuButton.onClick.AddListener(GameManager.LoadMainMenu);
        GameManager.OnNewGameRecord += OnNewGameRecord;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.OnNewGameRecord -= OnNewGameRecord;
    }

    private void OnNewGameRecord(int score)
    {
        recordText.enabled = true;
    }
}
