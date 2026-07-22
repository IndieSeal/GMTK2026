using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        Candle.OnCandleBurntOut += OnGameLost;
    }

    private void OnGameLost()
    {
        Debug.Log("womp womp, you lost :p");
    }
}