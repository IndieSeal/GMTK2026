using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Candle : MonoBehaviour
{
    public static event Action OnCandleBurntOut;
    
    [Header("Candle Duration")]
    [SerializeField] private float maxCandleDuration = 10;
    public float CandleWasteMultiplier { get; set; } = 1;
    private float candleTimer;

    void Awake()
    {
        volume = Instantiate(volume);
        volume.profile.TryGet(out vignette);

        baseGlobalLightColor = globalLight.color;

        candleTimer = maxCandleDuration;
    }

    void Update()
    {
        AddCandleDuration(-Time.deltaTime * CandleWasteMultiplier);

        HandleDarkness();
        HandleVignette();
    }

    
    #region Candle

    private float GetCandleValue() => Mathf.Lerp(1, 0, candleTimer / maxCandleDuration);

    public void AddCandleDuration(float value)
    {
        candleTimer = Mathf.Clamp(candleTimer + value, 0, maxCandleDuration);

        if(candleTimer <= 0) OnCandleBurntOut?.Invoke();
    }

    #endregion
    
    #region Visuals
    
    [Header("Visuals")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private Volume volume;
    private Color baseGlobalLightColor;

    [Header("Darkness")]
    [SerializeField] private Color darknessColor = Color.black;
    private Vignette vignette;

    private void HandleDarkness()
    {
        Color currentGlobalLightColor = globalLight.color;
        currentGlobalLightColor.r = Mathf.Lerp(baseGlobalLightColor.r, darknessColor.r, GetCandleValue());
        currentGlobalLightColor.g = Mathf.Lerp(baseGlobalLightColor.g, darknessColor.g, GetCandleValue());
        currentGlobalLightColor.b = Mathf.Lerp(baseGlobalLightColor.b, darknessColor.b, GetCandleValue());
        globalLight.color = currentGlobalLightColor;
    }

    private void HandleVignette()
    {
        vignette.intensity.value = Mathf.Lerp(0.2f, 1, GetCandleValue());
    }

    //gotta make the animation smoother later!! (just use a new variable which is current value, and feed that to the lerps)

    #endregion
}