using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class KidInput : Singleton<KidInput>
{
    public const string MOVE_ACTION_NAME = "Move";
    public const string FIRE_ACTION_NAME = "Fire";
    public const string DASH_ACTION_NAME = "Dash";
    
    [SerializeField] private PlayerInput playerInput;
    public InputAction MoveAction { get; private set; }
    public InputAction FireAction { get; private set; }
    public InputAction DashAction { get; private set; }

    private Dictionary<InputAction, List<Action>> onStartActions = new Dictionary<InputAction, List<Action>>();
    private Dictionary<InputAction, List<Action>> onPerformActions = new Dictionary<InputAction, List<Action>>();
    private Dictionary<InputAction, List<Action>> onStopActions = new Dictionary<InputAction, List<Action>>();

    protected override void Awake()
    {
        base.Awake();
        
        MoveAction = playerInput.actions.FindAction(MOVE_ACTION_NAME);
        FireAction = playerInput.actions.FindAction(FIRE_ACTION_NAME);
        DashAction = playerInput.actions.FindAction(DASH_ACTION_NAME);
    }

    void OnEnable()
    {
        SetupAction(MoveAction);
        SetupAction(FireAction);
        SetupAction(DashAction);
    }

    void OnDisable()
    {
        UnsetupAction(MoveAction);
        UnsetupAction(FireAction);
        UnsetupAction(DashAction);
    }

    private void SetupAction(InputAction inputAction)
    {
        onStartActions.TryAdd(inputAction, new List<Action>());
        onPerformActions.TryAdd(inputAction, new List<Action>());
        onStopActions.TryAdd(inputAction, new List<Action>());

        inputAction.started += ActionStarted;
        inputAction.performed += ActionPerformed;
        inputAction.canceled += ActionStopped;
    }

    private void UnsetupAction(InputAction inputAction)
    {
        onStartActions.Remove(inputAction);
        onPerformActions.Remove(inputAction);
        onStopActions.Remove(inputAction);

        inputAction.started -= ActionStarted;
        inputAction.performed -= ActionPerformed;
        inputAction.canceled -= ActionStopped;
    }

    private void ActionStarted(CallbackContext ctx)
    {
        var list = onStartActions[ctx.action];
        list.ForEach(x => x?.Invoke());
    }

    private void ActionPerformed(CallbackContext ctx)
    {
        var list = onPerformActions[ctx.action];
        list.ForEach(x => x?.Invoke());
    }

    private void ActionStopped(CallbackContext ctx)
    {
        var list = onStopActions[ctx.action];
        list.ForEach(x => x?.Invoke());
    }

    public void SubscribeToInputAction(InputAction inputAction, Action onStart, Action onPerform, Action onStop)
    {
        if (onStartActions.ContainsKey(inputAction) && onStart != null) onStartActions[inputAction].Add(onStart);
        if (onPerformActions.ContainsKey(inputAction) && onPerform != null) onPerformActions[inputAction].Add(onPerform);
        if (onStopActions.ContainsKey(inputAction) && onStop != null) onStopActions[inputAction].Add(onStop);
    }

    public void UnsubscribeToInputAction(InputAction inputAction, Action onStart, Action onPerform, Action onStop)
    {
        if (onStartActions.ContainsKey(inputAction) && onStart != null) onStartActions[inputAction].Remove(onStart);
        if (onPerformActions.ContainsKey(inputAction) && onPerform != null) onPerformActions[inputAction].Remove(onPerform);
        if (onStopActions.ContainsKey(inputAction) && onStop != null) onStopActions[inputAction].Remove(onStop);
    }
}