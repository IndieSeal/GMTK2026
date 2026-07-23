using UnityEngine;

public class RotateAround : MonoBehaviour
{
    [SerializeField] private float radius = 1;
    [SerializeField] private Vector2 offset = Vector2.up;
    private bool canRotate = true;

    void Update()
    {
        if(!canRotate) return;
        
        Vector3 mousePosition = Utilities.Get2DMouseWorldPosition();
        Vector2 direction = mousePosition - transform.parent.position;

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();
            transform.localPosition = offset + (direction * radius);
        }
    }

    void OnEnable()
    {
        HidingSpot.OnPlayerHid += PlayerHidInSpot;
        HidingSpot.OnPlayerExit += PlayerExitSpot;
    }

    void OnDisable()
    {
        HidingSpot.OnPlayerHid -= PlayerHidInSpot;
        HidingSpot.OnPlayerExit -= PlayerExitSpot;
    }

    private void PlayerHidInSpot(HidingSpot hidingSpot)
    {
        canRotate = false;
    }

    private void PlayerExitSpot(HidingSpot hidingSpot)
    {
        canRotate = true;
    }
}