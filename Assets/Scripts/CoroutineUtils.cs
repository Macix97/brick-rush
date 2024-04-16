using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public static class CoroutineUtils
{
    public static IEnumerator WaitForSeconds(float time)
    {
        for (float i = 0.0f; i < time; i += Time.deltaTime) yield return null;
    }

    public static IEnumerator WaitForSecondsRealtime(float time)
    {
        for (float i = 0.0f; i < time; i += Time.unscaledDeltaTime) yield return null;
    }

    public static IEnumerator WaitForCompletion(this Task task)
    {
        while (!task.IsCompleted) yield return null;
    }
}
