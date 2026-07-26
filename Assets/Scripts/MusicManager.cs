using FMODUnity;
using FMOD.Studio;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{
    [SerializeField] private StudioEventEmitter music;
    [SerializeField] private StudioEventEmitter chaseMusic;
    private Candle candle;

    protected override void Awake()
    {
        base.Awake();

        if(Instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnEnable()
    {
        PlayerMovement.OnPlayerDeath += ChangeDeathParameter;

        ChaseSequence.OnChaseSequenceStart += SilenceMusic;
        ChaseSequence.OnChaseSequenceTP += StartChaseMusic;
    }

    void OnDisable()
    {
        PlayerMovement.OnPlayerDeath -= ChangeDeathParameter;

        ChaseSequence.OnChaseSequenceStart -= SilenceMusic;
        ChaseSequence.OnChaseSequenceTP -= StartChaseMusic;
    }

    void Update()
    {
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
        music.Stop();
    }

    private void ChangeDeathParameter()
    {
        chaseMusic.Stop();
        if(!music.IsPlaying()) music.Play();
        RuntimeManager.StudioSystem.setParameterByName("Death", 1);
    }
}