using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviourPersistentSingleton<CameraManager>
{
    private Camera mainCamera;
    private static readonly Vector2 centerTopViewport = new(0.5F, 1.0F);
    private static readonly Vector2 rightTopViewport = new(1.0F, 1.0F);

    private static Camera MainCamera => Instance.mainCamera;

    protected override void Awake()
    {
        base.Awake();
        mainCamera = GetComponent<Camera>();
    }

    public static Vector2 GetRightTopScreenPosition()
    {
        return MainCamera.ViewportToScreenPoint(rightTopViewport);
    }

    public static Vector3 GetCenterTopWorldPosition()
    {
        return MainCamera.ViewportToWorldPoint(centerTopViewport);
    }

    public static Vector2 GetScreenSize(Renderer renderer) => GetScreenSize(renderer.bounds);

    public static Vector2 GetScreenSize(Bounds bounds)
    {
        return (MainCamera.WorldToScreenPoint(bounds.min) - MainCamera.WorldToScreenPoint(bounds.max)).Abs();
    }
}
