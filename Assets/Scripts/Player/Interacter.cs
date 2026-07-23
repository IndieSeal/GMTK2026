using UnityEngine;

public class Interacter : MonoBehaviour
{
    protected KidInput Input => KidInput.Instance;
    
    private IInteractable currentInteractable;

    void Start()
    {
        Input.SubscribeToInputAction(Input.InteractAction, OnInteract, null, null);
    }

    private void OnInteract()
    {
        if(currentInteractable == null) return;

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

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IInteractable newInteractable))
        {
            newInteractable.OnExitRange();
            if(currentInteractable == newInteractable) currentInteractable = null;
        }
    }
}