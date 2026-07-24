using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    public event Action OnOpen;
    public event Action OnCrossed;
    public event Action OnClose;

    [SerializeField] private HitReceiver openCollider;
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private HitReceiver crossDoorCollider;

    void OnEnable()
    {
        openCollider.OnAnyHit += TryOpen;
        crossDoorCollider.OnAnyHit += Crossed;
    }

    void OnDisable()
    {
        openCollider.OnAnyHit -= TryOpen;
        crossDoorCollider.OnAnyHit -= Crossed;
    }

    private void TryOpen(GameObject collision)
    {
        if(!collision.TryGetComponent(out PlayerMovement player)) return;

        Open();
    }

    public void Open()
    {
        openCollider.gameObject.SetActive(false);
        doorCollider.enabled = false;
        OnOpen?.Invoke();
    }

    public void Crossed(GameObject collision)
    {
        if(!collision.TryGetComponent(out PlayerMovement player)) return;
        
        crossDoorCollider.gameObject.SetActive(false);
        OnCrossed?.Invoke();
    }

    public void Close(bool canBeOpenedManually = true)
    {
        doorCollider.enabled = true;
        OnClose?.Invoke();
    }
}