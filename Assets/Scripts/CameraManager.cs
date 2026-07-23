using UnityEngine;

//Thanks unity forums: https://discussions.unity.com/t/move-camera-towards-mouse-in-2d/836269/7
public class CameraManager : MonoBehaviour
{
    public KidInput Input => KidInput.Instance;
    
    [Header("Target")]
    [SerializeField] private Transform follow;
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
    }

    void OnDisable()
    {
        HidingSpot.OnPlayerHid -= PlayerHidInSpot;
        HidingSpot.OnPlayerExit -= PlayerExitSpot;
    }

    void Start()
    {
        Input.SubscribeToInputAction(Input.MoveAction, null, ChangeMovingDirection, ChangeMovingDirection);
    }

    private void ChangeMovingDirection()
    {
        targetOffsetX = Input.MoveAction.ReadValue<Vector2>().x;
    }

    void Update()
    {
        HandleCameraMovement();
    }

    private void PlayerHidInSpot(HidingSpot hidingSpot)
    {
        useOffset = false;
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
        SetPosition(Vector2.Lerp(follow.position, targetPosition, sensitivity));
    }

    private void SetPosition(Vector3 position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }
}