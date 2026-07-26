using FMODUnity;
using UnityEngine;

public class CandleRevival : MonoBehaviour
{
    [SerializeField] private float candleRestoreValue = 50;
    [SerializeField] private Room assignedToRoom;

    [Header("Audio")]
    [SerializeField] private EventReference RefuelEvent;

    void Awake()
    {
        if (assignedToRoom != null)
        {
            assignedToRoom.OnRoomClearedEvent += ShowUp;
            gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (assignedToRoom != null) assignedToRoom.OnRoomClearedEvent -= ShowUp;
    }

    private void ShowUp()
    {
        gameObject.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.TryGetComponent(out Candle candle)) return;

        RuntimeManager.PlayOneShot(RefuelEvent, transform.position);
        candle.AddCandleDuration(candleRestoreValue, true);
        gameObject.SetActive(false);
    }
}