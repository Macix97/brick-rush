using System;
using System.Collections;
using UnityEngine;

// TODO: Leaderboard scroll view
public class GameManager : MonoBehaviourPersistentSingleton<GameManager>
{
    [SerializeField] private GameSettings settings;

    private bool isUserChecked;
    private float recentTimeScale;
    private GameState currentState;
    private GameState recentState;

    public static GameSettings Settings => Instance.settings;
    public static float RecentTimeScale { get => Instance.recentTimeScale; private set => Instance.recentTimeScale = value; }
    public static GameState CurrentState { get => Instance.currentState; private set => Instance.currentState = value; }
    public static GameState RecentState { get => Instance.recentState; private set => Instance.recentState = value; }
    public static bool IsUserChecked { get => Instance.isUserChecked; private set => Instance.isUserChecked = value; }

    public static event Action OnUpdate;
    public static event Action<int> OnNewGameRecord;
    public static event Action<float> OnTimeScaleChanged;

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeSceneLoaded()
    {
        SceneLoader.LoadScene(SceneName.PersistentMenagers, true);
    }
#endif

    protected override void Awake()
    {
        base.Awake();
        LevelManager.OnStateChanged += OnLevelStateChanged;
        SceneLoader.OnScenePrepared += OnScenePrepared;
        SceneLoader.OnSceneLoadingEnded += OnSceneLoadingEnded;
        SceneLoader.OnSceneLoadingStarted += OnSceneLoadingStarted;
        InputManager.OnBackButtonClicked += OnBackButtonClicked;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        LevelManager.OnStateChanged -= OnLevelStateChanged;
        SceneLoader.OnScenePrepared -= OnScenePrepared;
        SceneLoader.OnSceneLoadingEnded -= OnSceneLoadingEnded;
        SceneLoader.OnSceneLoadingStarted -= OnSceneLoadingStarted;
        InputManager.OnBackButtonClicked -= OnBackButtonClicked;
    }

    private IEnumerator Start()
    {
        OnScenePrepared(SceneLoader.ActiveScene);
        OnSceneLoadingEnded(SceneLoader.ActiveScene);
        yield return OnGameLoop();
    }

    private IEnumerator OnGameLoop()
    {
        while (true)
        {
            yield return null;
            OnUpdate?.Invoke();
        }
    }

    private void OnLevelStateChanged(LevelState levelState)
    {
        switch (levelState)
        {
            case LevelState.GameOver:
                SetState(GameState.GameOver);
                break;
            case LevelState.NextStage:
                UIManager.OpenWindow<StageWindow>();
                break;
            case LevelState.Play:
                UIManager.CloseWindow<StageWindow>();
                break;
        }
    }

    private void OnSceneLoadingStarted(SceneName sceneName)
    {
        SetState(GameState.Loading);
    }

    private void OnBackButtonClicked()
    {
        switch (currentState)
        {
            case GameState.Play:
            case GameState.PlayStart:
                SetState(GameState.Pause);
                break;
            case GameState.Pause:
            case GameState.GameOver:
                LoadMainMenu();
                break;
            default:
                QuitGame();
                break;
        }
    }

    private void OnScenePrepared(SceneName sceneName)
    {
        SetNormalTimeScale();
        switch (sceneName)
        {
            case SceneName.Start:
                SetState(GameState.Initialization);
                break;
            case SceneName.Game:
                SetState(GameState.PlayStart);
                break;
            case SceneName.MainMenu:
                if (!IsUserChecked) break;
                SetState(GameState.MainMenu);
                break;
        }
    }

    private void OnSceneLoadingEnded(SceneName sceneName)
    {
        switch (sceneName)
        {
            case SceneName.MainMenu:
                if (IsUserChecked) break;
                FirebaseManager.CheckIsUserAsync(isUser =>
                {
                    if (isUser)
                        SetState(GameState.MainMenu);
                    else
                        SetState(GameState.CreateAccount);
                    IsUserChecked = true;
                });
                break;
        }
    }

    public static void SetPreviousState() => SetState(RecentState);

    public static void QuitGame() => Application.Quit();

    public static void LoadGame() => SceneLoader.StartSceneLoading(SceneName.Game);

    public static void LoadMainMenu() => SceneLoader.StartSceneLoading(SceneName.MainMenu);

    public static void SetState(GameState newState)
    {
        if (CurrentState == newState) return;
        RecentState = CurrentState;
        CurrentState = newState;
        OnExitingState();
        OnEnteringState();
    }

    private static void OnExitingState()
    {
        switch (RecentState)
        {
            case GameState.Pause:
                SetTimeScale(RecentTimeScale);
                UIManager.CloseWindow<PauseWindow>();
                break;
            case GameState.CreateAccount:
                UIManager.CloseWindow<AccountWindow>();
                break;
            case GameState.MainMenu:
                UIManager.CloseWindow<MainMenuWindow>();
                break;
            case GameState.Leaderboard:
                UIManager.CloseWindow<LeaderboardWindow>();
                break;
        }
    }

    private static void OnEnteringState()
    {
        switch (CurrentState)
        {
            case GameState.PlayStart:
                LevelManager.StartLevel();
                SetState(GameState.Play);
                break;
            case GameState.GameOver:
                FirebaseManager.GetUserScoreAsync(score =>
                {
                    if (LevelManager.Score > score)
                        OnNewGameRecord?.Invoke(LevelManager.Score);
                    FirebaseManager.SetUserScoreAsync(LevelManager.Score);
                });
                UIManager.OpenWindow<GameOverWindow>();
                break;
            case GameState.Pause:
                StopTimeScale();
                UIManager.OpenWindow<PauseWindow>();
                break;
            case GameState.CreateAccount:
                UIManager.OpenWindow<AccountWindow>();
                break;
            case GameState.MainMenu:
                UIManager.CloseWindow<ConnectingWindow>(IsUserChecked);
                UIManager.OpenWindow<MainMenuWindow>(IsUserChecked);
                break;
            case GameState.Leaderboard:
                UIManager.OpenWindow<LeaderboardWindow>();
                break;
        }
    }

    public static void StopTimeScale() => SetTimeScale(0.0f);

    public static void SetNormalTimeScale() => SetTimeScale(1.0f);

    public static void SetTimeScale(float timeScale)
    {
        if (timeScale == Time.timeScale) return;
        RecentTimeScale = Time.timeScale;
        Time.timeScale = timeScale;
        OnTimeScaleChanged?.Invoke(timeScale);
    }
}
