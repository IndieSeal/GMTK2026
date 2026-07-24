using System;
using TMPro;
using UnityEngine;

public class HidingSpot : MonoBehaviour, IInteractable
{
    public static event Action<HidingSpot> OnPlayerHid;
    public static event Action<HidingSpot> OnPlayerExit;
    
    [SerializeField] private TMP_Text interactPrompt;
    [SerializeField] private Transform hidingTransform;
    private Transform playerTransform;
    private Vector3 lastPosition;
    
    public void OnEnterRange()
    {
        ChangePrompt();
    }
    public void OnExitRange()
    {
        interactPrompt.gameObject.SetActive(false);
    }

    private void ChangePrompt()
    {
        string prompt = playerTransform == null ? "enter" : "exit";

        interactPrompt.gameObject.SetActive(true);
        interactPrompt.text = $"(E) to {prompt}";
    }

    public void OnInteracted()
    {
        if(playerTransform == null)
        {
            playerTransform = FindAnyObjectByType<PlayerMovement>().transform;
            lastPosition = playerTransform.position;
            
            playerTransform.position = hidingTransform.position;

            ChangePrompt();
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