using UnityEngine;

public class Dummy : MonoBehaviour
{
    [SerializeField] private HealthSystem healthSystem;

    void OnEnable()
    {
        healthSystem.OnCharacterDeath += OnDeath;
    }

    void OnDisable()
    {
        healthSystem.OnCharacterDeath -= OnDeath;
    }

    private void OnDeath()
    {
        gameObject.SetActive(false);
    }
}