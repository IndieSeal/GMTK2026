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

    //We can do pools later, but it does require a restart system for the health system,
    //and I prefer working on other features over a small performance feature on the limited time-frame
    private void OnDeath()
    {
        gameObject.SetActive(false);
    }
}