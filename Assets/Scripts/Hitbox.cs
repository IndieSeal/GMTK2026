using UnityEngine;

public class Hitbox : HitReceiver
{
    [SerializeField] private HealthSystem healthSystem;
    public HealthSystem HealthSystem => healthSystem;
}