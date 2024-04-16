using UnityEngine;
using TMPro;

public class StageWindow : UIWindow
{
    [SerializeField, Min(0.1f)] private float speedMultiplier = 1.0f;
    [SerializeField] private TextMeshProUGUI stageValueText;

    protected override void Awake()
    {
        base.Awake();
        Animator.speed *= speedMultiplier;
    }

    public override void Open(bool instant)
    {
        base.Open(instant);
        stageValueText.text = LevelManager.CurrentStage.ToString();
    }
}
