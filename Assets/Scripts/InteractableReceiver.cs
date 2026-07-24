using System;
using UnityEngine;

public class InteractableReceiver : MonoBehaviour, IInteractable
{
    public event Action OnEnter;
    public event Action OnExit;
    public event Action OnInteract;
    
    public void OnEnterRange() => OnEnter?.Invoke();
    public void OnExitRange() => OnExit?.Invoke();
    public void OnInteracted() => OnInteract?.Invoke();
}