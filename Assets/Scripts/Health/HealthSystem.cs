using System;
using System.Collections;
using FMODUnity;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event Action OnCharacterHealed;
    public event Action OnCharacterDamaged;
    public event Action OnCharacterDeath;
    
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }

    [SerializeField] private Renderer mainRenderer;
    [SerializeField] private Renderer hitRenderer;

    [SerializeField] private bool playSound = false;
    [SerializeField] private EventReference HitSound;

    public bool IsInmune { get; set; } = false;

    void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    void Update()
    {
        if(mainRenderer != null && mainRenderer.TryGetComponent(out SpriteRenderer spr))
        {
            hitRenderer.GetComponent<SpriteRenderer>().sprite = spr.sprite;
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, maxHealth);
        OnCharacterHealed?.Invoke();
    }

    public void Damage(int amount)
    {
        if(IsInmune)
        {
            Debug.Log("immunity");
            return;
        }
        
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, maxHealth);
        OnCharacterDamaged?.Invoke();

        if(hitRenderer != null) StartCoroutine(OnHit());
        if(playSound) RuntimeManager.PlayOneShot(HitSound);
        if(CurrentHealth <= 0) OnCharacterDeath?.Invoke();
    }

    float hitTime = 0.2f;
    private IEnumerator OnHit()
    {
        float maxValue = hitTime / 2;
        
        if(mainRenderer != null) hitRenderer.gameObject.SetActive(true);
        
        float value = 0;
        while (value < maxValue)
        {
            value += Time.deltaTime;
            hitRenderer.material.SetFloat("_HitEffectBlend", Mathf.Lerp(0, 1, value / maxValue));
            yield return null;
        }
        
        hitRenderer.material.SetFloat("_HitEffectBlend", 1);
        yield return null;

        value = 0;
        while (value < maxValue)
        {
            value += Time.deltaTime;
            hitRenderer.material.SetFloat("_HitEffectBlend", Mathf.Lerp(1, 0, value / maxValue));
            yield return null;
        }

        hitRenderer.material.SetFloat("_HitEffectBlend", 0);

        if(mainRenderer != null) hitRenderer.gameObject.SetActive(false);
    }
}