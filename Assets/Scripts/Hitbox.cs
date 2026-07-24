using UnityEngine;

public class Hitbox : HitReceiver
{
    [SerializeField] private HealthSystem healthSystem;
    public HealthSystem HealthSystem => healthSystem;

    void OnEnable()
    {
        if(TryGetComponent(out Collider2D collider)) collider.enabled = true;
    }

    void OnDisable()
    {
        if(TryGetComponent(out Collider2D collider)) collider.enabled = false;
    }
}