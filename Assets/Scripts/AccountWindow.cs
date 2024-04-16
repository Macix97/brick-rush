using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AccountWindow : UIWindow
{
    [SerializeField] private Button createButton;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TextMeshProUGUI failText;

    protected override void Awake()
    {
        base.Awake();
        failText.enabled = false;
        createButton.onClick.AddListener(OnCreateButtonClicked);
        nameInputField.onValueChanged.AddListener(OnNameInputFieldChanged);
    }

    protected override void Start()
    {
        base.Start();
        OnNameInputFieldChanged(nameInputField.text);
    }

    private void OnCreateButtonClicked()
    {
        SetInteractable(false);
        FirebaseManager.CreateAccountAsync(nameInputField.text, success =>
        {
            if (success)
            {
                failText.enabled = false;
                GameManager.SetState(GameState.MainMenu);
            }
            else
            {
                failText.enabled = true;
            }
            SetInteractable(true);
        });
    }

    private void OnNameInputFieldChanged(string text)
    {
        createButton.interactable = text.Length >= GameManager.Settings.MinAccountNameLength;
    }
}
