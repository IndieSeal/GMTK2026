using FMODUnity;
using FMOD.Studio;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private EventInstance musicInstance;
    
    [SerializeField] private StudioEventEmitter chaseMusic;
    private Candle candle;
    private bool isDead;

    void Awake()
    {
        CreateNewInstance();
    }

    void OnEnable()
    {
        StartMusic();
        
        PlayerMovement.OnPlayerDeath += ChangeDeathParameter;

        ChaseSequence.OnChaseSequenceStart += SilenceMusic;
        ChaseSequence.OnChaseSequenceTP += StartChaseMusic;
    }

    void OnDisable()
    {
        SilenceMusic();

        PlayerMovement.OnPlayerDeath -= ChangeDeathParameter;

        ChaseSequence.OnChaseSequenceStart -= SilenceMusic;
        ChaseSequence.OnChaseSequenceTP -= StartChaseMusic;
    }

    private void StartMusic()
    {
        musicInstance.start();
    }

    void Update()
    {
        if(isDead) return;
        if(candle == null)
        {
            candle = FindAnyObjectByType<Candle>();
            return;
        }
        
        float candleValue = candle.GetCandleValue();
        int stressLevel = 1;

        if(candleValue < 0.9f) stressLevel = Mathf.CeilToInt(candle.GetCandleValue() * 3);
        else stressLevel = 4;

        RuntimeManager.StudioSystem.setParameterByName("Stress", stressLevel);
    }

    private void StartChaseMusic()
    {
        chaseMusic.Play();
    }

    private void SilenceMusic()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
    }

    private void ChangeDeathParameter()
    {
        RuntimeManager.StudioSystem.setParameterByName("Stress", 6);
        isDead = true;
        chaseMusic.Stop();
    }

    private void CreateNewInstance()
    {
        musicInstance = RuntimeManager.CreateInstance("event:/Music/Main MX");
    }
}