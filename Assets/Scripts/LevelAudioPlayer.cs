using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LevelAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip brickClip;
    [SerializeField] private AudioClip stageClip;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private Vector2 pitchRange = new(0.9f, 1.1f);

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        LevelManager.OnStateChanged += OnLevelStateChanged;
        BrickComponent.OnStateChanged += OnBrickStateChanged;
    }

    private void OnDestroy()
    {
        LevelManager.OnStateChanged -= OnLevelStateChanged;
        BrickComponent.OnStateChanged -= OnBrickStateChanged;
    }

    private void OnBrickStateChanged(BrickComponent brick)
    {
        if (brick.State != BrickState.Deactivating) return;
        PlayAudio(brickClip, true);
    }

    private void OnLevelStateChanged(LevelState levelState)
    {
        switch (levelState)
        {
            case LevelState.NextStage:
                PlayAudio(stageClip);
                break;
            case LevelState.GameOver:
                PlayAudio(gameOverClip);
                break;
        }
    }

    private void PlayAudio(AudioClip audioClip, bool randomPitch = false)
    {
        audioSource.clip = audioClip;
        audioSource.pitch = randomPitch ? Random.Range(pitchRange.x, pitchRange.y) : 1.0f;
        audioSource.Play();
    }
}
