using System.Collections;
using FMODUnity;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private float minSpeed = 7;
    [SerializeField] private float maxSpeed = 10;
    [SerializeField] private float chaseSpeed = 15;
    [SerializeField] private float pauseDelay = 2;

    [SerializeField] private float speedyDistance = 30;

    [SerializeField] private ParticleSystem spawnParticles;
    [SerializeField] private EventReference screamSound;
    
    private PlayerMovement playerInstance;
    private Candle candleInstance;

    private Vector2 targetPosition = new(30, 10);
    private bool isHiding;
    private bool justDelay;
    private bool isFinalChasing;
    
    void Awake()
    {
        RandomizePosition();

        playerInstance = FindAnyObjectByType<PlayerMovement>();       
        candleInstance = FindAnyObjectByType<Candle>();
    }

    void OnEnable()
    {
        HidingSpot.OnPlayerHid += GoToRandomPosition;
        HidingSpot.OnPlayerExit += StopHiding;

        Door.OnRoomChanged += OnRoomChanged;
        PlayerMovement.OnPlayerDeath += OnPlayerDeath;

        Door.OnRoomChangedEnd += ChasedRoomChanged;

        ChaseSequence.OnChaseSequenceStart += ChaseSequenceStarted;
        ChaseSequence.OnChaseSequenceTP += SpawnIn;
        ChaseSequence.OnChaseSequenceChase += StartCHASE;
    }

    void OnDisable()
    {
        HidingSpot.OnPlayerHid -= GoToRandomPosition;
        HidingSpot.OnPlayerExit -= StopHiding;

        Door.OnRoomChanged -= OnRoomChanged;
        PlayerMovement.OnPlayerDeath -= OnPlayerDeath;

        Door.OnRoomChangedEnd -= ChasedRoomChanged;

        ChaseSequence.OnChaseSequenceStart -= ChaseSequenceStarted;
        ChaseSequence.OnChaseSequenceTP -= SpawnIn;
        ChaseSequence.OnChaseSequenceChase -= StartCHASE;
    }

    private void OnPlayerDeath()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        Move();

        if (!isFinalChasing)
        {
            if(justDelay || candleInstance.GetCandleValue() < 0.4f) return;
        }

        if(isHiding) return;
        targetPosition = playerInstance.transform.position;
    }

    private void ChaseSequenceStarted()
    {
        StopAllCoroutines();

        justDelay = true;
        RandomizePosition();
    }

    private void SpawnIn()
    {
        Vector2 targetPosition = PlayerMovement.instance.transform.position + new Vector3(-2, 2);
        spawnParticles.transform.position = targetPosition;
        spawnParticles.Play();

        justDelay = true;
        this.targetPosition = targetPosition;

        RuntimeManager.PlayOneShot(screamSound);

        StartCoroutine(Appear(targetPosition));
    }

    private IEnumerator Appear(Vector2 appear)
    {
        yield return new WaitForSeconds(0.3f);
        transform.position = appear;
    }

    private void StartCHASE()
    {
        StartCoroutine(ChaseDelay());
    }

    private IEnumerator ChaseDelay()
    {
        if(!isFinalChasing) yield return new WaitForSeconds(1.5f);
        justDelay = false;
        isFinalChasing = true;
    }

    private void Move()
    {
        float velocity = Mathf.InverseLerp(0.4f, 1, candleInstance.GetCandleValue());
        velocity = Mathf.Lerp(minSpeed, maxSpeed, velocity);

        if(isFinalChasing) velocity = chaseSpeed;

        if(!isFinalChasing && Vector2.Distance(transform.position, playerInstance.transform.position) > speedyDistance) velocity *= 2;
        
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, velocity * Time.deltaTime);
    }

    private void GoToRandomPosition(HidingSpot hidingSpot)
    {
        isHiding = true;
        RandomizePosition();
    }

    private void RandomizePosition()
    {
        targetPosition = new Vector2(Random.Range(.5f, 20), Random.Range(20, 38));
    }

    private Coroutine roomChangedCoroutine;

    private void OnRoomChanged()
    {
        if (isFinalChasing) return;
        
        targetPosition += Vector2.one * 5;
        justDelay = true;
        
        if(roomChangedCoroutine != null) StopCoroutine(roomChangedCoroutine);
        roomChangedCoroutine = StartCoroutine(OnRoomChangedCoroutine());
    }

    private void ChasedRoomChanged()
    {
        if(!isFinalChasing) return;

        transform.position = playerInstance.transform.position + Vector3.up * 10;
    }

    private IEnumerator OnRoomChangedCoroutine()
    {
        yield return new WaitForSeconds(pauseDelay);
        justDelay = false;
        roomChangedCoroutine = null;
    }

    private void StopHiding(HidingSpot hidingSpot)
    {
        isHiding = false;
    }
}