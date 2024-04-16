using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviourPersistentSingleton<SceneLoader>
{
    private bool isLoading;
    private SceneName sceneToLoad;

    public static SceneName ActiveScene => (SceneName)SceneManager.GetActiveScene().buildIndex;

    public static event Action<SceneName> OnSceneLoadingStarted;
    public static event Action<SceneName> OnScenePrepared;
    public static event Action<SceneName> OnSceneLoadingEnded;

    private IEnumerator Start()
    {
        while (true)
        {
            yield return isLoading ? OnLoading() : null;
        }
    }

    private IEnumerator OnLoading()
    {
        LoadingScreen.SetCanvasActive(true);
        yield return LoadingScreen.OnFading(1.0f);
        LoadingScreen.SetViewGroupActive(true);
        yield return LoadingScreen.OnFading(0.0f);
        yield return LoadSceneAsync(SceneName.Loading, true);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)SceneName.Loading));
        AsyncOperation asyncOperation = LoadSceneAsync(sceneToLoad, false);
        while (!asyncOperation.isDone)
        {
            LoadingScreen.SetProgressBar(asyncOperation.progress);
            yield return null;
        }
        yield return null;
        OnScenePrepared?.Invoke(sceneToLoad);
        LoadingScreen.SetProgressBar(1.0f);
        yield return LoadingScreen.OnFading(1.0f);
        LoadingScreen.SetViewGroupActive(false);
        yield return LoadingScreen.OnFading(0.0f);
        LoadingScreen.SetCanvasActive(false);
        Instance.isLoading = false;
        OnSceneLoadingEnded?.Invoke(sceneToLoad);
    }

    public static void ReloadScene() => StartSceneLoading(ActiveScene);

    public static void LoadScene(SceneName sceneName, bool additive = false)
    {
        SceneManager.LoadScene((int)sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
    }

    public static AsyncOperation LoadSceneAsync(SceneName sceneName, bool additive = false)
    {
        return SceneManager.LoadSceneAsync((int)sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
    }

    public static void StartSceneLoading(SceneName sceneName)
    {
        if (Instance.isLoading) return;
        Instance.sceneToLoad = sceneName;
        Instance.isLoading = true;
        OnSceneLoadingStarted?.Invoke(Instance.sceneToLoad);
    }
}
