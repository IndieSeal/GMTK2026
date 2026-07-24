using FMODUnity;
using FMOD.Studio;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private Candle candle;
    
    void Awake()
    {
        candle = FindAnyObjectByType<Candle>();
    }

    void Update()
    {

        float candleValue = candle.GetCandleValue();
        int stressLevel = 0;

        if(candleValue < 0.9f) stressLevel = Mathf.CeilToInt(candle.GetCandleValue() * 3);
        else stressLevel = 4;
        Debug.Log(stressLevel);

        RuntimeManager.StudioSystem.setParameterByName("Stress", stressLevel);
    }
}