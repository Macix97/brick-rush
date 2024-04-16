using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour
{
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Image audioImage;
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(OnButtonClicked);
        AudioManager.OnAudioVolumeChanged += SetAudioSprite;
    }

    private void Start()
    {
        SetAudioSprite(AudioListener.volume);
    }

    private void OnDestroy()
    {
        AudioManager.OnAudioVolumeChanged -= SetAudioSprite;
    }

    private void SetAudioSprite(float volume)
    {
        audioImage.overrideSprite = volume == 0.0f ? offSprite : null;
    }

    private void OnButtonClicked()
    {
        AudioManager.ToggleAudioVolume();
    }
}
