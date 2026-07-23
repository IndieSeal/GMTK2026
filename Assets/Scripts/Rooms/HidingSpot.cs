using System;
using UnityEngine;

public class HidingSpot : MonoBehaviour, IInteractable
{
    public static event Action<HidingSpot> OnPlayerHid;
    public static event Action<HidingSpot> OnPlayerExit;
    
    [SerializeField] private Transform hidingTransform;
    private Transform playerTransform;
    private Vector3 lastPosition;
    
    public void OnEnterRange(){}
    public void OnExitRange(){}

    public void OnInteracted()
    {
        if(playerTransform == null)
        {
            playerTransform = FindAnyObjectByType<PlayerMovement>().transform;
            lastPosition = playerTransform.position;

            OnPlayerHid?.Invoke(this);
        }
        else
        {
            playerTransform.position = lastPosition;
            playerTransform = null;
            
            OnPlayerExit?.Invoke(this);
        }
    }
}