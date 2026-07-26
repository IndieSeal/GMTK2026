using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [Header("Shooting UI")]
    [SerializeField] private TMP_Text bulletCount_Txt;
    private Candle candle;

    void Awake()
    {
        candle = FindAnyObjectByType<Candle>();
    }

    void OnEnable()
    {
        PlayerMovement.OnPlayerDeath += PlayerDied;

        ChaseSequence.OnChaseSequenceStart += PlayerDied;
    }

    void OnDisable()
    {
        PlayerMovement.OnPlayerDeath -= PlayerDied;

        ChaseSequence.OnChaseSequenceStart -= PlayerDied;
    }

    private void PlayerDied()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        string startText = !candle.IsReloading ? candle.BulletCount.ToString() : "..."; 
        bulletCount_Txt.text = $"{startText}/{candle.MaxBulletCount}";
    }
}