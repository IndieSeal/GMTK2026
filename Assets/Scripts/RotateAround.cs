using UnityEngine;

public class RotateAround : MonoBehaviour
{
    [SerializeField] private float radius = 1;
    [SerializeField] private Vector2 offset = Vector2.up;

    void Update()
    {
        Vector3 mousePosition = Utilities.Get2DMouseWorldPosition();
        Vector2 direction = mousePosition - transform.parent.position;

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();
            transform.localPosition = offset + (direction * radius);
        }
    }
}