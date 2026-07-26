using System.Collections;
using UnityEngine;

//Thanks unity forums: https://discussions.unity.com/t/move-camera-towards-mouse-in-2d/836269/7
public class CameraManager : MonoBehaviour
{
    public KidInput Input => KidInput.Instance;
    
    [Header("Target")]
    [SerializeField] private Transform follow;
    [SerializeField] private Transform cameraShake;
    private Transform oldFollow;

    [Header("Movement")]
    [SerializeField] private float sensitivity = 0.15f;
    [SerializeField] float offsetMoveSpeed = 25f;
    private Vector2 targetPosition = Vector2.zero;
    private Vector3 cameraOffset = Vector2.zero;
    private float targetOffsetX = 0f;
    private bool useOffset = true;

    void OnEnable()
    {
        HidingSpot.OnPlayerHid += PlayerHidInSpot;
        HidingSpot.OnPlayerExit += PlayerExitSpot;

        PlayerMovement.OnPlayerDeath += OnCharacterDeath;
    }

    void OnDisable()
    {
        HidingSpot.OnPlayerHid -= PlayerHidInSpot;
        HidingSpot.OnPlayerExit -= PlayerExitSpot;

        PlayerMovement.OnPlayerDeath -= OnCharacterDeath;
    }

    void Start()
    {
        Input.SubscribeToInputAction(Input.MoveAction, null, ChangeMovingDirection, ChangeMovingDirection);
    }

    private void OnCharacterDeath()
    {
        StartCoroutine(ShakeCoroutine(1f, 0.2f, 1f, 0.2f));
        DisableOffset();
    }

    private void DisableOffset()
    {
        useOffset = false;
    }

    private void ChangeMovingDirection()
    {
        targetOffsetX = Input.MoveAction.ReadValue<Vector2>().x;
    }

    void Update()
    {
        HandleCameraMovement();
        if(cameraShake != null) SetLocalPosition(cameraShake, Random.insideUnitCircle * shake);
    }

    private void PlayerHidInSpot(HidingSpot hidingSpot)
    {
        DisableOffset();
        oldFollow = follow;
        follow = hidingSpot.transform;
    }

    private void PlayerExitSpot(HidingSpot hidingSpot)
    {
        follow = oldFollow;
        useOffset = true;
    }

    private void HandleCameraMovement()
    {
        Rect screenRect = new Rect(0f, 0f, Screen.width, Screen.height);
        cameraOffset.x = Mathf.MoveTowards(cameraOffset.x, targetOffsetX, offsetMoveSpeed * Time.fixedDeltaTime);

        if(screenRect.Contains(Utilities.GetMousePosition())) targetPosition = Utilities.Get2DMouseWorldPosition() + (Vector2)cameraOffset;

        if(!useOffset) targetPosition = follow.position;
        SetPosition(transform, Vector2.Lerp(follow.position, targetPosition, sensitivity));
    }

    private void SetPosition(Transform t, Vector3 position)
    {
        t.position = new Vector3(position.x, position.y, t.position.z);
    }

    private void SetLocalPosition(Transform t, Vector3 position)
    {
        t.localPosition = new Vector3(position.x, position.y, t.localPosition.z);
    }

    float shake;

    public IEnumerator ShakeCoroutine(float shakeAmount, float fadeInTime, float length, float fadeOutTime)
    {
        float time = 0;
        while (time < fadeInTime)
        {
            float t = time / fadeInTime;
            shake = Mathf.Lerp(0, shakeAmount, t);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(length);
        time = 0;
        while (time < fadeOutTime)
        {
            float t = time / fadeInTime;
            shake = Mathf.Lerp(0, shakeAmount, 1 - t);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        shake = 0;
    }
}