using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event Action OnCharacterHealed;
    public event Action OnCharacterDamaged;
    public event Action OnCharacterDeath;
    
    [SerializeField] private int maxHealth = 3;
    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }

    void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, maxHealth);
        OnCharacterHealed?.Invoke();
    }

    //We can do pools later, but it does require a restart system for the health system,
    //and I prefer working on other features over a small performance feature on the limited time-frame
    public void Damage(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, maxHealth);
        OnCharacterDamaged?.Invoke();
        
        if(CurrentHealth <= 0) OnCharacterDeath?.Invoke();
    }
}