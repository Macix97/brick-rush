using System;
using UnityEngine;

public class Timer : IDisposable
{
    [SerializeField] private bool elapsed;
    [SerializeField] private float targetTime;
    [SerializeField] private float currentTime;

    private Action action;

    public Timer(float targetTime, Action action)
    {
        elapsed = false;
        this.targetTime = targetTime;
        currentTime = targetTime;
        this.action = action;
        GameManager.OnUpdate += OnUpdate;
    }

    public void Dispose()
    {
        GameManager.OnUpdate -= OnUpdate;
    }

    public void Restart(float targetTime = default, Action action = default)
    {
        if (targetTime != default) this.targetTime = targetTime;
        if (action != default) this.action = action;
        currentTime = this.targetTime;
        Resume();
    }

    private void OnUpdate()
    {
        if (elapsed) return;
        currentTime -= Time.deltaTime;
        if (currentTime <= 0.0f)
        {
            Stop();
            action?.Invoke();
        }
    }

    public void Stop()
    {
        elapsed = true;
    }

    public void Resume()
    {
        elapsed = false;
    }
}
