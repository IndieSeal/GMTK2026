using System;
using System.Collections;
using FMODUnity;
using TMPro;
using UnityEngine;

public class Door : MonoBehaviour
{
    public event Action OnOpen;
    public event Action OnCrossed;
    public event Action OnClose;

    [Header("Components")]
    [SerializeField] private Door linkedDoor; 
    [SerializeField] private Animator animator;

    [SerializeField] private InteractableReceiver openCollider;
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private HitReceiver crossDoorCollider;
    public bool IsClosed { get => isClosed; set
        {
            isClosed = value;
            Debug.Log(isClosed);
            animator.SetBool("IsClosed", IsClosed);
        }}
    private bool isClosed = true;

    [SerializeField] private Transform startPosition;
    public Transform tpTransform => startPosition.transform;

    [SerializeField] private TMP_Text interactPrompt;

    [Header("Audio")]
    [SerializeField] private EventReference OpenEvent;
    [SerializeField] private EventReference CloseEvent;
    private Coroutine fadeCoroutine;

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

    private void MoveTowardsDoor(GameObject go)
    {
        if(!go.TryGetComponent(out PlayerMovement player) || IsClosed || fadeCoroutine != null) return;

        linkedDoor.IsClosed = IsClosed;
        fadeCoroutine = StartCoroutine(TransitionManager.Instance.FadeCoroutine(1, () => MoveTowardsDoorCoroutine(player), () => MoveTowardsDoorFinal(player)));
    }

    private void MoveTowardsDoorCoroutine(PlayerMovement player)
    {
        player.transform.position = linkedDoor.tpTransform.position;
    }

    private void MoveTowardsDoorFinal(PlayerMovement player) => fadeCoroutine = null;

    private void InteractWithDoor()
    {
        if(IsClosed) Open();
        else Close();
    }

    private void ShowPrompt()
    {
        string prompt = IsClosed ? "open" : "close";

        interactPrompt.gameObject.SetActive(true);
        interactPrompt.text = $"(E) to {prompt}";
    }

    private void HidePrompt()
    {
        interactPrompt.gameObject.SetActive(false);
    }

    public void Open(bool forced = false)
    {
        IsClosed = false;

        if(forced)
        {
            doorCollider.enabled = true;
            openCollider.gameObject.SetActive(true);
        }

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
        bool wasPreviouslyClosed = IsClosed;
        IsClosed = true;

        if(forced)
        {
            doorCollider.enabled = false;
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
        doorCollider.enabled = true;
        openCollider.gameObject.SetActive(true);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        MoveTowardsDoor(collision.gameObject);
    }
}