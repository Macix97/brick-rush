using UnityEngine;

public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (!Instance) Instance = this as T;
        else Destroy(gameObject);
    }

    protected virtual void OnDestroy() { }
}
