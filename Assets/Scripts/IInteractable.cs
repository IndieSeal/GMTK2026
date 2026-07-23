using UnityEngine;

public interface IInteractable
{
    void OnEnterRange();
    void OnExitRange();
    void OnInteracted();
}