using System;
using System.Collections.Generic;

public class UIManager : MonoBehaviourPersistentSingleton<UIManager>
{
    private readonly Dictionary<Type, UIWindow> allWindows = new();
    private readonly List<UIWindow> windowsBuffer = new();

    private static Dictionary<Type, UIWindow> AllWindows => Instance.allWindows;

    protected override void Awake()
    {
        base.Awake();
        UIWindow.OnStarting += AddWindow;
        UIWindow.OnDestroyed += RemoveWindow;
        UIWindow.OnStateChanged += OnWindowStateChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UIWindow.OnStarting -= AddWindow;
        UIWindow.OnDestroyed -= RemoveWindow;
        UIWindow.OnStateChanged -= OnWindowStateChanged;
    }

    private void AddWindow(UIWindow window)
    {
        allWindows.TryAdd(window.Type, window);
        windowsBuffer.Add(window);
    }

    private void RemoveWindow(UIWindow window)
    {
        allWindows.Remove(window.Type);
        windowsBuffer.Remove(window);
    }

    public static void OpenWindow<T>(bool instant = false) where T : UIWindow
    {
        if (!AllWindows.TryGetValue(typeof(T), out UIWindow window)) return;
        window.Open(instant);
    }

    public static void CloseWindow<T>(bool instant = false) where T : UIWindow
    {
        if (!AllWindows.TryGetValue(typeof(T), out UIWindow window)) return;
        window.Close(instant);
    }

    private void OnWindowStateChanged(UIWindow window)
    {
        if (!window.IsOpen) return;
        windowsBuffer.SetAsFirst(window);
        int count = windowsBuffer.Count;
        for (int i = 0; i < count; i++)
            windowsBuffer[i].SortingOrder = count - i;
    }
}
