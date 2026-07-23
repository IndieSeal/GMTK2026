using UnityEngine;

public class RotateAround : MonoBehaviour
{
    [SerializeField] private float radius = 1;

    void Update()
    {
        Vector3 mousePosition = Utilities.Get2DMouseWorldPosition();
        Vector2 direction = mousePosition - transform.parent.position;

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();
            transform.localPosition = direction * radius;
        }
    }
}