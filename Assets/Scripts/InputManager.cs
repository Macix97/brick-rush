using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventSystem), typeof(StandaloneInputModule))]
public class InputManager : MonoBehaviourPersistentSingleton<InputManager>
{
    private EventSystem eventSystem;
    private StandaloneInputModule inputModule;

    private static EventSystem EventSystem => Instance.eventSystem;
    private static StandaloneInputModule InputModule => Instance.inputModule;

    public static event Action OnBackButtonClicked;

    protected override void Awake()
    {
        base.Awake();
        eventSystem = GetComponent<EventSystem>();
        inputModule = GetComponent<StandaloneInputModule>();
        GameManager.OnUpdate += OnUpdate;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.OnUpdate -= OnUpdate;
    }

    private void OnUpdate()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        OnBackButtonClicked?.Invoke();
    }
}
