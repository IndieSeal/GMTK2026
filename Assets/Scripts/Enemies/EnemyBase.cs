using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public event Action<EnemyBase> OnKilled;
    
    [Header("Enemy Base")]
    [SerializeField] private HealthSystem health;
    [SerializeField] private Hitbox hitReceiver;

    void OnEnable()
    {
        health.OnCharacterDeath += OnEnemyKilled;
    }

    void OnDisable()
    {
        health.OnCharacterDeath -= OnEnemyKilled;
    }

    public virtual void OnStartBehaviour()
    {
        hitReceiver.enabled = true;

        gameObject.SetActive(true);
    }

    public virtual void OnStopBehaviour()
    {
        hitReceiver.enabled = false;

        gameObject.SetActive(false);
    }

    public virtual void OnEnemyKilled()
    {
        OnKilled?.Invoke(this);

        OnStopBehaviour();
    }
}