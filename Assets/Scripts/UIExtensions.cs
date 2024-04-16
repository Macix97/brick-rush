using UnityEngine;
using UnityEngine.UI;

public static class UIExtensions
{
    public static void SetAlpha(this Image image, float alpha)
    {
        Color color = image.color;
        image.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(alpha));
    }

    public static void SetActive(this CanvasGroup canvasGroup, bool active)
    {
        canvasGroup.alpha = active ? 1.0f : 0.0f;
        canvasGroup.blocksRaycasts = active;
        canvasGroup.interactable = active;
    }
}
