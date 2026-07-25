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
        playerInstance = FindAnyObjectByType<PlayerMovement>();       
        candleInstance = FindAnyObjectByType<Candle>();
    }

    void OnEnable()
    {
        HidingSpot.OnPlayerHid += GoToRandomPosition;
        HidingSpot.OnPlayerExit += StopHiding;

        Door.OnRoomChanged += OnRoomChanged;
    }

    void OnDisable()
    {
        HidingSpot.OnPlayerHid -= GoToRandomPosition;
        HidingSpot.OnPlayerExit -= StopHiding;

        Door.OnRoomChanged -= OnRoomChanged;
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

        if(Vector2.Distance(transform.position, targetPosition) > speedyDistance) velocity = maxSpeed * 2;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, velocity * Time.deltaTime);
    }

    private void GoToRandomPosition(HidingSpot hidingSpot)
    {
        isHiding = true;
        targetPosition = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));
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