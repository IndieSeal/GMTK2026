using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public event Action<EnemyBase> OnKilled;
    
    [Header("Enemy Base")]
    [SerializeField] private HealthSystem health;
    [SerializeField] private Hitbox hitbox;

    void Awake()
    {
        OnStopBehaviour();
    }

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
        hitbox.enabled = true;

        gameObject.SetActive(true);
    }

    public virtual void OnStopBehaviour()
    {
        hitbox.enabled = false;
    }

    public virtual void OnEnemyKilled()
    {
        OnKilled?.Invoke(this);

        OnStopBehaviour();
        gameObject.SetActive(false);
    }
}