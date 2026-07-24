using System;
using FMODUnity;
using TMPro;
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

    [SerializeField] private TMP_Text interactPrompt;

    [Header("Audio")]
    [SerializeField] private EventReference OpenEvent;
    [SerializeField] private EventReference CloseEvent;

    void Awake()
    {
        HidePrompt();
    }

    void OnEnable()
    {
        openCollider.OnEnter += ShowPrompt;
        openCollider.OnExit += HidePrompt;
        
        openCollider.OnInteract += InteractWithDoor;
        crossDoorCollider.OnAnyHit += Crossed;
    }

    void OnDisable()
    {
        openCollider.OnEnter -= ShowPrompt;
        openCollider.OnExit -= HidePrompt;

        openCollider.OnInteract -= InteractWithDoor;
        crossDoorCollider.OnAnyHit -= Crossed;
    }

    private void InteractWithDoor()
    {
        if(isClosed) Open();
        else Close();
    }

    private void ShowPrompt()
    {
        string prompt = isClosed ? "open" : "close";

        interactPrompt.gameObject.SetActive(true);
        interactPrompt.text = $"(E) to {prompt}";
    }

    private void HidePrompt()
    {
        interactPrompt.gameObject.SetActive(false);
    }

    public void Open(bool forced = false)
    {
        doorCollider.enabled = false;
        isClosed = false;

        if(forced) openCollider.gameObject.SetActive(true);

        RuntimeManager.PlayOneShot(OpenEvent, transform.position);
        
        ShowPrompt();
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
        bool wasPreviouslyClosed = isClosed;
        isClosed = true;

        if(forced)
        {
            openCollider.gameObject.SetActive(false);
            HidePrompt();
        }
        else ShowPrompt();

        OnClose?.Invoke();

        if(wasPreviouslyClosed) return;

        RuntimeManager.PlayOneShot(CloseEvent, transform.position);
    }

    public void RemoveForcedDoor()
    {
        openCollider.gameObject.SetActive(true);
    }
}