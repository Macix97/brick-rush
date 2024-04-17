using System.Collections;
using UnityEngine;

public class GameLauncher : MonoBehaviour
{
    [SerializeField, Min(0.5f)] private float splashScreenTime = 2.0f;

    private IEnumerator Start()
    {
        yield return CoroutineUtils.WaitForSecondsRealtime(splashScreenTime);
        yield return SceneLoader.LoadSceneAsync(SceneName.PersistentMenagers, true);
        GameManager.LoadMainMenu();
    }
}
