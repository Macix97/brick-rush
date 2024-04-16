using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardWindow : UIWindow
{
    [SerializeField] private string displayFormat = "{0}. {1}: {2}";
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI userText;
    [SerializeField] private TextMeshProUGUI leaderboardText;

    private readonly StringBuilder stringBuilder = new();
    private readonly List<FirebaseUser> users = new();

    protected override void Awake()
    {
        base.Awake();
        userText.text = string.Empty;
        leaderboardText.text = string.Empty;
        backButton.onClick.AddListener(() => GameManager.SetState(GameState.MainMenu));
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        FirebaseManager.OnDatabaseChanged -= UpdateLeaderboard;
    }

    public override void Open(bool instant = false)
    {
        base.Open(instant);
        UpdateLeaderboard();
        FirebaseManager.OnDatabaseChanged += UpdateLeaderboard;
    }

    public override void Close(bool instant = false)
    {
        base.Close(instant);
        FirebaseManager.OnDatabaseChanged -= UpdateLeaderboard;
    }

    private void UpdateLeaderboard()
    {
        FirebaseManager.GetAllUsersAsync(users, () =>
        {
            users.Sort();
            stringBuilder.Clear();
            int count = users.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                string userData = string.Format(displayFormat, count - i, users[i].Name, users[i].Score);
                if (users[i].Name == FirebaseManager.UserData.Name)
                    userText.text = userData;
                stringBuilder.AppendLine(userData);
            }
            leaderboardText.text = stringBuilder.ToString();
        });
    }
}
