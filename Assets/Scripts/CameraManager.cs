using UnityEngine;

//Thanks unity forums: https://discussions.unity.com/t/move-camera-towards-mouse-in-2d/836269/7
public class CameraManager : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform follow;

    [Header("Movement")]
    [SerializeField] private float sensitivity = 0.15f;
    [SerializeField] float offsetMoveSpeed = 25f;
    private Vector2 targetPosition = Vector2.zero;
    private Vector3 cameraOffset = Vector2.zero;
    private float targetOffsetX = 0f;

    void Start()
    {
        KidInput.Instance.SubscribeToInputAction(KidInput.Instance.MoveAction, null, ChangeMovingDirection, ChangeMovingDirection);
    }

    private void ChangeMovingDirection()
    {
        targetOffsetX = KidInput.Instance.MoveAction.ReadValue<Vector2>().x;
    }

    void Update()
    {
        HandleCameraMovement();
    }

    private void HandleCameraMovement()
    {
        Rect screenRect = new Rect(0f, 0f, Screen.width, Screen.height);
        cameraOffset.x = Mathf.MoveTowards (cameraOffset.x, targetOffsetX, offsetMoveSpeed * Time.fixedDeltaTime);

        if(screenRect.Contains(Utilities.GetMousePosition())) targetPosition = Utilities.Get2DMouseWorldPosition() + (Vector2)cameraOffset;
        SetPosition(Vector2.Lerp(follow.position, targetPosition, sensitivity));
    }

    private void SetPosition(Vector3 position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }
}