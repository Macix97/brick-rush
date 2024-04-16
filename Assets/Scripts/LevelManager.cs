using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class LevelManager : MonoBehaviourSingleton<LevelManager>
{
    [SerializeField] private BrickComponent brickPrefab;

    private LevelState state;
    private int score;
    private int currentStage;
    private int currentBrickCount;
    private int remainingBrickCount;
    private int targetBrickCount;
    private int brickColumnCount;
    private float targetBrickSpeed;
    private float brickSpawnInterval;
    private float brickWorldSpread;
    private float brickScreenSpread;
    private Timer timer;
    private Vector2 brickStartPoint;
    private Vector2 brickWorldSize;
    private Vector2 brickScreenSize;
    private IObjectPool<BrickComponent> brickPool;
    private readonly List<Vector3> brickPointsPool = new();
    private readonly List<Vector3> brickStartPoints = new();

    public static int Score => Instance.score;
    public static int CurrentStage => Instance.currentStage;
    public static float TargetBrickSpeed => Instance.targetBrickSpeed;
    public static LevelState State => Instance.state;

    public static event Action<int> OnScoreUpdated;
    public static event Action<LevelState> OnStateChanged;

    private const int StartStage = 1;

    protected override void Awake()
    {
        base.Awake();
        currentStage = StartStage;
        BrickComponent.OnStateChanged += OnBrickStateChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        timer.Dispose();
        BrickComponent.OnStateChanged -= OnBrickStateChanged;
    }

    private void Start()
    {
        timer = new Timer(GameManager.Settings.LevelBreakTime, OnTimerElapsed);
        brickWorldSize = brickPrefab.SpriteRenderer.bounds.size;
        brickWorldSpread = brickWorldSize.x * GameManager.Settings.BrickSpaceFactor;
        brickScreenSize = CameraManager.GetScreenSize(brickPrefab.SpriteRenderer);
        brickStartPoint = CameraManager.GetCenterTopWorldPosition() + Vector3.up * brickWorldSpread;
        brickScreenSpread = brickScreenSize.x * GameManager.Settings.BrickSpaceFactor;
        brickPool = new ObjectPool<BrickComponent>(OnCreatePoolObject, OnGetPoolObject, OnReleasePoolObject, OnDestroyPoolObject);
        brickColumnCount = Mathf.FloorToInt((CameraManager.GetRightTopScreenPosition().x - brickScreenSpread) / (brickScreenSize.x + brickScreenSpread));
        if (brickColumnCount.IsEven()) brickColumnCount--;
        CollectBrickStartPoints();
    }

    public static void StartLevel()
    {
        Instance.timer.Restart(GameManager.Settings.StartPlayDelay);
    }

    private void OnTimerElapsed()
    {
        switch (state)
        {
            case LevelState.Unspecified:
                OnUnspecifiedTimerElapsed();
                break;
            case LevelState.Play:
                OnPlayTimerElapsed();
                break;
            case LevelState.NextStage:
                OnNextStageTimerElapsed();
                break;
        }
    }

    private void OnUnspecifiedTimerElapsed()
    {
        SetState(LevelState.NextStage);
    }

    private void OnPlayTimerElapsed()
    {
        BrickComponent brick = brickPool.Get();
        brick.SetState(BrickState.Falling);
        brick.transform.position = GetBrickStartPoint();
        currentBrickCount++;
        remainingBrickCount--;
        if (remainingBrickCount > 0)
            timer.Restart(brickSpawnInterval);
    }

    private void OnNextStageTimerElapsed()
    {
        SetState(LevelState.Play);
    }

    private void SetState(LevelState newState)
    {
        if (state == newState) return;
        state = newState;
        switch (state)
        {
            case LevelState.Play:
                OnLevelPlayState();
                break;
            case LevelState.NextStage:
                OnLevelNextStageState();
                break;
        }
        OnStateChanged?.Invoke(state);
    }

    private void OnLevelPlayState()
    {
        if (currentStage == StartStage)
        {
            targetBrickCount = GameManager.Settings.StartBrickCount;
            targetBrickSpeed = GameManager.Settings.StartBrickSpeed;
            brickSpawnInterval = GameManager.Settings.StartBrickSpawnInterval;
        }
        else
        {
            targetBrickCount += GameManager.Settings.BrickCountDelta;
            targetBrickSpeed += GameManager.Settings.BrickSpeedDelta;
            brickSpawnInterval = Mathf.Max(brickSpawnInterval - GameManager.Settings.BrickSpawnIntervalDelta, GameManager.Settings.MinBrickSpawnInterval);
        }
        currentStage++;
        remainingBrickCount = targetBrickCount;
        timer.Restart(brickSpawnInterval);
    }

    private void OnLevelNextStageState()
    {
        timer.Restart(GameManager.Settings.LevelBreakTime);
    }

    private void OnBrickStateChanged(BrickComponent brick)
    {
        switch (brick.State)
        {
            case BrickState.Idle:
                OnBrickIdleState(brick);
                break;
            case BrickState.Deactivating:
                OnBrickDeactivatingState();
                break;
            case BrickState.Fallen:
                OnBrickFallenState();
                break;
        }
    }

    private void OnBrickIdleState(BrickComponent brick)
    {
        brickPool.Release(brick);
    }

    private void OnBrickDeactivatingState()
    {
        IncreaseScore();
        DecreaseCurrentBrickCount();
    }

    private void OnBrickFallenState()
    {
        SetState(LevelState.GameOver);
    }

    private void IncreaseScore()
    {
        score++;
        OnScoreUpdated?.Invoke(score);
    }

    private void DecreaseCurrentBrickCount()
    {
        currentBrickCount--;
        if (remainingBrickCount == 0 && currentBrickCount == 0)
            SetState(LevelState.NextStage);
    }

    private BrickComponent OnCreatePoolObject()
    {
        BrickComponent brick = Instantiate(brickPrefab);
        brick.name = nameof(BrickComponent);
        return brick;
    }

    private void OnGetPoolObject(BrickComponent brick)
    {
        brick.gameObject.SetActive(true);
    }

    private void OnReleasePoolObject(BrickComponent brick)
    {
        brick.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(BrickComponent brick)
    {
        Destroy(brick.gameObject);
    }

    private Vector2 GetBrickStartPoint()
    {
        if (brickPointsPool.Count == 0)
        {
            brickPointsPool.AddRange(brickStartPoints);
            brickPointsPool.Shuffle();
        }
        return brickPointsPool.Random(true);
    }

    private void CollectBrickStartPoints()
    {
        float xPosition = 0.0f;
        for (int i = 0; i < brickColumnCount; i++)
        {
            float offset = i.IsEven() ? xPosition : -xPosition;
            brickStartPoints.Add(brickStartPoint + Vector2.right * offset);
            if (i.IsEven()) xPosition += brickWorldSize.x + brickWorldSpread;
        }
    }
}
