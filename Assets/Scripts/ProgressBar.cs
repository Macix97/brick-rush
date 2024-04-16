using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    public void SetFill(float fillAmount) => fillImage.fillAmount = Mathf.Clamp01(fillAmount);
}
