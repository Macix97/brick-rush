using UnityEngine;

[CreateAssetMenu(fileName = nameof(GameSettings))]
public class GameSettings : ScriptableObject
{
    [SerializeField, Min(0.5f)] private float startPlayDelay = 0.5f;
    [SerializeField, Min(1)] private int startBrickCount = 1;
    [SerializeField, Min(1)] private int brickCountDelta = 1;
    [SerializeField, Min(0.5f)] private float levelBreakTime = 1.0f;
    [SerializeField, Min(0.1f)] private float startBrickSpeed = 1.0f;
    [SerializeField, Min(1.0f)] private float startBrickSpawnInterval = 2.0f;
    [SerializeField, Min(0.1f)] private float minBrickSpawnInterval = 0.5f;
    [SerializeField, Min(0.1f)] private float brickSpeedDelta = 0.5f;
    [SerializeField, Min(0.2f)] private float brickAccelerationTime = 0.2f;
    [SerializeField, Min(0.05f)] private float brickSpawnIntervalDelta = 0.1f;
    [SerializeField, Range(0.1f, 5.0f)] private float brickSpaceFactor = 0.5f;
    [SerializeField, Min(2)] private int minAccountNameLength = 2;

    public float StartPlayDelay => startPlayDelay;
    public int StartBrickCount => startBrickCount;
    public int BrickCountDelta => brickCountDelta;
    public float LevelBreakTime => levelBreakTime;
    public float StartBrickSpeed => startBrickSpeed;
    public float StartBrickSpawnInterval => startBrickSpawnInterval;
    public float MinBrickSpawnInterval => minBrickSpawnInterval;
    public float BrickSpeedDelta => brickSpeedDelta;
    public float BrickAccelerationTime => brickAccelerationTime;
    public float BrickSpawnIntervalDelta => brickSpawnIntervalDelta;
    public float BrickSpaceFactor => brickSpaceFactor;
    public int MinAccountNameLength => minAccountNameLength;
}
