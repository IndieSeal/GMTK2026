using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using uPools;

public class Candle : MonoBehaviour
{
    public static event Action OnCandleBurntOut;

    public KidInput Input => KidInput.Instance;

    void Awake()
    {
        volume = Instantiate(volume);
        volume.profile.TryGet(out vignette);

        baseGlobalLightColor = globalLight.color;
        lerpedGlobalLightColor = baseGlobalLightColor;

        candleTimer = maxCandleDuration;
        BulletCount = maxBulletCount;

        maxCandleLightIntensity = candleLight.intensity;
        maxCandleLightOuter = candleLight.pointLightOuterRadius;
    }

    void OnEnable()
    {
        HidingSpot.OnPlayerHid += PlayerHidInSpot;
        HidingSpot.OnPlayerExit += PlayerExitSpot;
    }

    void OnDisable()
    {
        HidingSpot.OnPlayerHid -= PlayerHidInSpot;
        HidingSpot.OnPlayerExit -= PlayerExitSpot;
    }

    private void PlayerHidInSpot(HidingSpot hidingSpot)
    {
        if(shotDelayCoroutine != null) StopCoroutine(shotDelayCoroutine);
        CanShoot = false;
    }

    private void PlayerExitSpot(HidingSpot hidingSpot)
    {
        CanShoot = true;
    }

    void Start()
    {
        Input.SubscribeToInputAction(Input.FireAction, HandleShooting, null, null);
    }

    void Update()
    {
        AddCandleDuration(-Time.deltaTime * CandleWasteMultiplier);

        HandleDarkness();
        HandleCandleLight();
        HandleVignette();
    }

    #region Candle Manager

    [Header("Candle Duration")]
    [SerializeField] private float maxCandleDuration = 10;
    public float CandleWasteMultiplier { get; set; } = 1;
    private float candleTimer;

    public float GetCandleValue() => Mathf.Lerp(1, 0, candleTimer / maxCandleDuration);

    public void AddCandleDuration(float value)
    {
        candleTimer = Mathf.Clamp(candleTimer + value, 0, maxCandleDuration);
        if(candleTimer <= 0) OnCandleBurntOut?.Invoke();
    }

    #endregion

    #region Candle Gun

    [Header("Shooting")]
    [SerializeField] private int maxBulletCount = 15;
    [SerializeField] private float shotDelay = 0.1f;
    public int MaxBulletCount => maxBulletCount;
    public int BulletCount { get; private set; }
    public bool CanShoot { get; private set; } = true;

    [Header("Reloading")]
    [SerializeField] private float reloadTime = 1.6f;
    private Coroutine reloadCoroutine;
    private Coroutine shotDelayCoroutine;
    public bool IsReloading => reloadCoroutine != null;

    [Header("Bullets")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField]private float bulletVelocity = 5;
    [Space]
    [SerializeField] private Transform centerTransform;

    private void HandleShooting()
    {
        if(reloadCoroutine != null || !CanShoot) return;
        
        GameObject instance = SharedGameObjectPool.Rent(bulletPrefab, centerTransform.position, Quaternion.identity);
        shotDelayCoroutine = StartCoroutine(HandleShotDelay());
        
        BulletCount--;
        if(BulletCount <= 0) StartReload();
        
        if(instance.TryGetComponent(out Rigidbody2D rb)){
            rb.linearVelocity = (Utilities.Get2DMouseWorldPosition() - (Vector2)centerTransform.position).normalized * bulletVelocity;
        }
    }

    private void StartReload()
    {
        if(reloadCoroutine != null) return;

        reloadCoroutine = StartCoroutine(ReloadSequence());
    }

    private IEnumerator ReloadSequence()
    {   
        yield return new WaitForSeconds(reloadTime);

        BulletCount = maxBulletCount;
        reloadCoroutine = null;
    }

    private IEnumerator HandleShotDelay()
    {
        CanShoot = false;
        yield return new WaitForSeconds(shotDelay);
        CanShoot = true;
    }

    #endregion
    
    #region Visuals
    
    [Header("Visuals")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private Light2D candleLight;
    [SerializeField] private Volume volume;
    private float maxCandleLightIntensity = 1;
    private float maxCandleLightOuter;
    private Color baseGlobalLightColor;
    private Color lerpedGlobalLightColor;

    [Header("Transition Speeds")]
    [SerializeField] private float globalLightLerp = 2;

    [Header("Darkness")]
    [SerializeField] private Color darknessColor = Color.black;
    private Vignette vignette;

    private void HandleDarkness()
    {
        Color currentGlobalLightColor = Color.Lerp(baseGlobalLightColor, darknessColor, GetCandleValue());

        lerpedGlobalLightColor = Color.Lerp(lerpedGlobalLightColor, currentGlobalLightColor, globalLightLerp * Time.deltaTime);
        globalLight.color = lerpedGlobalLightColor;
    }

    private void HandleCandleLight()
    {
        candleLight.intensity = Mathf.Lerp(maxCandleLightIntensity, 0, GetCandleValue());
        candleLight.pointLightOuterRadius = Mathf.Lerp(maxCandleLightOuter, 0, GetCandleValue());
    }

    private void HandleVignette()
    {
        vignette.intensity.value = Mathf.Lerp(0.2f, 1, GetCandleValue());
    }

    //gotta make the animation smoother later!! (just use a new variable which is current value, and feed that to the lerps)

    #endregion
}