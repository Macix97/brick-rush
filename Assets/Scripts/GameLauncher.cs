using System.Collections;
using UnityEngine;

public class GameLauncher : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return CoroutineUtils.WaitForSecondsRealtime(GameManager.Settings.SplashScreenTime);
        yield return SceneLoader.LoadSceneAsync(SceneName.PersistentMenagers, true);
        GameManager.LoadMainMenu();
    }
}
