using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerHUD : UIBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreValueText;

    protected override void Awake()
    {
        base.Awake();
        LevelManager.OnScoreUpdated += SetScoreText;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        LevelManager.OnScoreUpdated -= SetScoreText;
    }

    private void SetScoreText(int score)
    {
        scoreValueText.text = score.ToString();
    }
}
