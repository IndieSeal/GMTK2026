using System;
using FMODUnity;
using UnityEngine;

public class Door : MonoBehaviour
{
    public event Action OnOpen;
    public event Action OnCrossed;
    public event Action OnClose;

    [SerializeField] private InteractableReceiver openCollider;
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private HitReceiver crossDoorCollider;
    private bool isClosed = true;

    [Header("Audio")]
    [SerializeField] private EventReference OpenEvent;
    [SerializeField] private EventReference CloseEvent;

    void OnEnable()
    {
        openCollider.OnInteract += InteractWithDoor;
        crossDoorCollider.OnAnyHit += Crossed;
    }

    void OnDisable()
    {
        openCollider.OnInteract -= InteractWithDoor;
        crossDoorCollider.OnAnyHit -= Crossed;
    }

    private void InteractWithDoor()
    {
        if(isClosed) Open();
        else Close();
    }

    public void Open(bool forced = false)
    {
        doorCollider.enabled = false;
        isClosed = false;

        if(forced) openCollider.gameObject.SetActive(true);

        RuntimeManager.PlayOneShot(OpenEvent, transform.position);
        
        OnOpen?.Invoke();
    }

    public void Crossed(GameObject collision)
    {
        if(!collision.TryGetComponent(out PlayerMovement player)) return;
        
        crossDoorCollider.gameObject.SetActive(false);
        OnCrossed?.Invoke();
    }

    public void Close(bool forced = false)
    {
        doorCollider.enabled = true;
        isClosed = true;

        if(forced) openCollider.gameObject.SetActive(false);

        RuntimeManager.PlayOneShot(CloseEvent, transform.position);
        
        OnClose?.Invoke();
    }

    public void RemoveForcedDoor()
    {
        openCollider.gameObject.SetActive(true);
    }
}