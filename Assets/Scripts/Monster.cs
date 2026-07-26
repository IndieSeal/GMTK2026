using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private float minSpeed = 7;
    [SerializeField] private float maxSpeed = 10;
    [SerializeField] private float pauseDelay = 2;

    [SerializeField] private float speedyDistance = 30;
    
    private PlayerMovement playerInstance;
    private Candle candleInstance;

    private Vector2 targetPosition = new(30, 10);
    private bool isHiding;
    private bool justDelay;
    
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
    }

    void OnDisable()
    {
        HidingSpot.OnPlayerHid -= GoToRandomPosition;
        HidingSpot.OnPlayerExit -= StopHiding;

        Door.OnRoomChanged -= OnRoomChanged;
        PlayerMovement.OnPlayerDeath -= OnPlayerDeath;
    }

    private void OnPlayerDeath()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        Move();
        
        if(justDelay || isHiding || candleInstance.GetCandleValue() < 0.4f) return;

        targetPosition = playerInstance.transform.position;
    }

    private void Move()
    {
        float velocity = Mathf.InverseLerp(0.4f, 1, candleInstance.GetCandleValue());
        velocity = Mathf.Lerp(minSpeed, maxSpeed, velocity);

        if(Vector2.Distance(transform.position, playerInstance.transform.position) > speedyDistance) velocity = maxSpeed * 2;
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

    private void OnRoomChanged()
    {
        targetPosition += Vector2.one * 5;
        justDelay = true;
        
        StopAllCoroutines();
        StartCoroutine(OnRoomChangedCoroutine());
    }

    private IEnumerator OnRoomChangedCoroutine()
    {
        yield return new WaitForSeconds(pauseDelay);
        justDelay = false;
    }

    private void StopHiding(HidingSpot hidingSpot)
    {
        isHiding = false;
    }
}