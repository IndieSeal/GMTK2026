using UnityEngine;

public class CandleRevival : MonoBehaviour
{
    [SerializeField] private float candleRestoreValue = 50;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.TryGetComponent(out Candle candle)) return;

        candle.AddCandleDuration(candleRestoreValue);
        gameObject.SetActive(false);
    }
}