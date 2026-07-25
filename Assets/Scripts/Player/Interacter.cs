using UnityEngine;

public class Interacter : MonoBehaviour
{
    protected KidInput Input => KidInput.Instance;
    
    private IInteractable currentInteractable;
    private bool canInteract = true;

    void OnEnable()
    {
        TransitionManager.TransitionStarted += OnTransitionStarted;
        TransitionManager.TransitionEnded += OnTransitionEnded;
    }

    void OnDisable()
    {
        TransitionManager.TransitionStarted -= OnTransitionStarted;
        TransitionManager.TransitionEnded -= OnTransitionEnded;
    }

    void Start()
    {
        Input.SubscribeToInputAction(Input.InteractAction, OnInteract, null, null);
    }

    private void OnTransitionStarted() => canInteract = false;
    private void OnTransitionEnded() => canInteract = true;

    private void OnInteract()
    {
        if(currentInteractable == null || !canInteract) return;

        currentInteractable.OnInteracted();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IInteractable newInteractable))
        {
            newInteractable.OnEnterRange();
            if(currentInteractable == null) currentInteractable = newInteractable;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(currentInteractable == null && collision.TryGetComponent(out IInteractable newInteractable))
        {
            newInteractable.OnEnterRange();
            if(currentInteractable == null) currentInteractable = newInteractable;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IInteractable newInteractable))
        {
            newInteractable.OnExitRange();
            if(currentInteractable == newInteractable) currentInteractable = null;
        }
    }
}