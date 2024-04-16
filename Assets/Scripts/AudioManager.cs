using System;
using UnityEngine;

public static class AudioManager
{
    public static event Action<bool> OnAudioPauseChanged;
    public static event Action<float> OnAudioVolumeChanged;

    public static void ToggleAudioVolume() => SetAudioVolume(AudioListener.volume == 0.0f ? 1.0f : 0.0f);

    public static void SetAudioPause(bool isPause)
    {
        if (AudioListener.pause == isPause) return;
        AudioListener.pause = isPause;
        OnAudioPauseChanged?.Invoke(isPause);
    }

    public static void SetAudioVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        AudioListener.volume = volume;
        OnAudioVolumeChanged?.Invoke(volume);
    }
}
