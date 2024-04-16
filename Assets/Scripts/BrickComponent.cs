using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BrickComponent : MonoBehaviour
{
    [SerializeField, Range(0.25f, 1.25f)] private float hidingDelay = 1.0f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private new Collider2D collider;
    [SerializeField] private new ParticleSystem particleSystem;

    private float velocity;
    private float currentSpeed;
    private float targetSpeed;
    private BrickState state;
    private Transform bodyTransform;

    public BrickState State => state;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    public static event Action<BrickComponent> OnStateChanged;

    private void Awake()
    {
        bodyTransform = transform;
    }

    private void OnEnable()
    {
        GameManager.OnUpdate += OnUpdate;
        LevelManager.OnStateChanged += OnLevelStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnUpdate -= OnUpdate;
        LevelManager.OnStateChanged -= OnLevelStateChanged;
    }

    private void OnUpdate()
    {
        UpdatePosition();
    }

    private void OnMouseDown()
    {
        if (state != BrickState.Falling) return;
        SetState(BrickState.Deactivating);
    }

    private void OnBecameInvisible()
    {
        if (state != BrickState.Falling) return;
        SetState(BrickState.Fallen);
    }

    private void OnParticleSystemStopped()
    {
        if (state != BrickState.Hidden) return;
        SetState(BrickState.Idle);
    }

    private void OnLevelStateChanged(LevelState levelState)
    {
        if (levelState != LevelState.GameOver) return;
        collider.enabled = false;
    }

    public void SetState(BrickState newState)
    {
        if (newState == state) return;
        state = newState;
        switch (state)
        {
            case BrickState.Deactivating:
                OnDeactivatingState();
                break;
            case BrickState.Falling:
                OnFallingState();
                break;
            case BrickState.Hidden:
                OnHiddenState();
                break;
        }
        OnStateChanged?.Invoke(this);
    }

    private void OnDeactivatingState()
    {
        particleSystem.Play();
        targetSpeed = 0.0f;
        collider.enabled = false;
        Invoke(nameof(SetHiddenState), hidingDelay);
    }

    private void OnFallingState()
    {
        currentSpeed = velocity = 0.0f;
        collider.enabled = true;
        targetSpeed = LevelManager.TargetBrickSpeed;
        SpriteRenderer.enabled = true;
        SpriteRenderer.color = Random.ColorHSV();
    }

    private void OnHiddenState()
    {
        SpriteRenderer.enabled = false;
    }

    private void SetHiddenState() => SetState(BrickState.Hidden);

    private void UpdatePosition()
    {
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref velocity, GameManager.Settings.BrickAccelerationTime);
        Vector3 position = bodyTransform.position;
        position.y -= currentSpeed * Time.deltaTime;
        bodyTransform.position = position;
    }
}
