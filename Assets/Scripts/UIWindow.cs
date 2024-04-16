using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator), typeof(Canvas), typeof(CanvasGroup))]
public abstract class UIWindow : UIBehaviour
{
    [SerializeField] private bool on;

    private Canvas canvas;
    private Animator animator;
    private CanvasGroup canvasGroup;
    private State state = State.Unspecified;

    protected Animator Animator => animator;
    public bool IsOpen => state == State.InstantOn || state == State.AnimatedOn;
    public Type Type { get; private set; }
    public int SortingOrder { get => canvas.sortingOrder; set => canvas.sortingOrder = value; }

    public static event Action<UIWindow> OnStarting;
    public static event Action<UIWindow> OnDestroyed;
    public static event Action<UIWindow> OnStateChanged;

    protected override void Awake()
    {
        base.Awake();
        Type = GetType();
        animator = GetComponent<Animator>();
        canvas = GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    protected override void Start()
    {
        base.Start();
        OnStarting?.Invoke(this);
        SetState(on ? State.InstantOn : State.InstantOff);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        OnDestroyed?.Invoke(this);
    }

    public virtual void Open(bool instant = false) => SetState(instant ? State.InstantOn : State.AnimatedOn);

    public virtual void Close(bool instant = false) => SetState(instant ? State.InstantOff : State.AnimatedOff);

    private void SetState(State newState)
    {
        if (state == newState) return;
        state = newState;
        SetInteractable(IsOpen);
        animator.SetInteger(nameof(State), (int)state);
        OnStateChanged?.Invoke(this);
    }

    public void SetInteractable(bool interactable)
    {
        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;
    }

    private enum State
    {
        Unspecified = -1,
        InstantOff = 0,
        AnimatedOff = 1,
        InstantOn = 2,
        AnimatedOn = 3,
    }
}
