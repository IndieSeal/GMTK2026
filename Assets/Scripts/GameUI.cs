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

    void Update()
    {
        string startText = !candle.IsReloading ? candle.BulletCount.ToString() : "..."; 
        bulletCount_Txt.text = $"{startText}/{candle.MaxBulletCount}";
    }
}