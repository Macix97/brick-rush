using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class LoadingScreen : MonoBehaviourPersistentSingleton<LoadingScreen>
{
    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private Image fadeImage;
    [SerializeField] private CanvasGroup viewGroup;
    [SerializeField, Min(0.1f)] private float fadingTime = 0.3f;
    [SerializeField, Min(0.1f)] private float breakTime = 0.1f;

    private Canvas canvas;

    private static Image FadeImage => Instance.fadeImage;
    private static float FadingTime => Instance.fadingTime;
    private static float BreakTime => Instance.breakTime;

    protected override void Awake()
    {
        base.Awake();
        canvas = GetComponent<Canvas>();
        FadeImage.SetAlpha(0.0f);
        SetViewGroupActive(false);
        SetCanvasActive(false);
    }

    public static void SetProgressBar(float progress)
    {
        Instance.progressBar.SetFill(progress);
    }

    public static void SetCanvasActive(bool active)
    {
        Instance.canvas.enabled = active;
    }

    public static void SetViewGroupActive(bool active)
    {
        SetProgressBar(0.0f);
        Instance.viewGroup.SetActive(active);
    }

    public static IEnumerator OnFading(float targetAlpha)
    {
        int sign = targetAlpha == 0.0f ? -1 : 1;
        while (FadeImage.color.a != targetAlpha)
        {
            float delta = Time.unscaledDeltaTime / FadingTime * sign;
            FadeImage.SetAlpha(Mathf.Clamp01(FadeImage.color.a + delta));
            yield return null;
        }
        yield return CoroutineUtils.WaitForSecondsRealtime(BreakTime);
    }
}
