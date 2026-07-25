using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    enum PlayerState
    {
        Normal,
        Hiding
    }
    
    public KidInput Input => KidInput.Instance;
    
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Header("Moving")]
    [SerializeField] private float movingSpeed = 5;
    private Vector2 currentInput;
    private Vector2 lastInput = Vector2.right;

    [Header("Dashing")]
    [SerializeField] private float dashVelocity = 5;
    [SerializeField] private float dashDuration = 1;
    [SerializeField] private float dashDelay = 1.3f;
    private Vector2 dashingDirection;
    private bool isDashing;
    private bool dashRecharging;
    private Coroutine dashCoroutine;

    private PlayerState playerState = PlayerState.Normal;

    void OnEnable()
    {
        HidingSpot.OnPlayerHid += OnPlayerHide;
        HidingSpot.OnPlayerExit += OnPlayerExitHiding;

        TransitionManager.TransitionStarted += StopMovement;
        TransitionManager.TransitionEnded += StartMovement;
    }

    void OnDisable()
    {
        HidingSpot.OnPlayerHid -= OnPlayerHide;
        HidingSpot.OnPlayerExit -= OnPlayerExitHiding;

        TransitionManager.TransitionStarted -= StopMovement;
        TransitionManager.TransitionEnded -= StartMovement;
    }

    void Start()
    {
        Input.SubscribeToInputAction(Input.MoveAction, null, ChangeMovingDirection, ChangeMovingDirection);
        Input.SubscribeToInputAction(Input.DashAction, StartDash, null, null);
    }

    void FixedUpdate()
    {
        if(playerState != PlayerState.Normal) return;
        
        HandleMove();
        HandleDash();
    }

    #region Movement

    private void HandleMove()
    {
        if(isDashing) return;

        rb.linearVelocity = (Vector3)currentInput * movingSpeed;

        if(playerState != PlayerState.Normal) return;
        
        animator.SetFloat("MovX", currentInput.x);
        animator.SetFloat("MovY", currentInput.y);
        animator.SetBool("IsMoving", currentInput.sqrMagnitude > 0.01f);

        if(currentInput.sqrMagnitude == 0)
        {
            animator.SetFloat("MovX", lastInput.x);
            animator.SetFloat("MovY", lastInput.y);
        }
    }

    private void ChangeMovingDirection()
    {
        if(currentInput.sqrMagnitude > 0.01f) lastInput = currentInput.normalized; 
        currentInput = Input.MoveAction.ReadValue<Vector2>().normalized;
    }

    #endregion
    #region Dashing

    private bool CanDash() => !isDashing && !dashRecharging && playerState == PlayerState.Normal;

    private void HandleDash()
    {
        if(!isDashing) return;
        
        rb.linearVelocity = dashingDirection * dashVelocity;
    }

    private void StartDash()
    {
        if(!CanDash()) return;

        dashingDirection = (currentInput.sqrMagnitude > 0.01f) ? currentInput : lastInput;
        dashCoroutine = StartCoroutine(DashCoroutine());

        StartCoroutine(DashDelayCoroutine());
    }

    private IEnumerator DashDelayCoroutine()
    {
        dashRecharging = true;
        yield return new WaitForSeconds(dashDelay);
        dashRecharging = false;
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        
        yield return new WaitForSeconds(dashDuration);

        StopDash();
    }

    private void StopDash()
    {
        rb.linearVelocity = Vector2.zero;
        isDashing = false;
        
        if(dashCoroutine == null) return;
        StopCoroutine(dashCoroutine);
        dashCoroutine = null;
    }

    #endregion

    public void StopMovement()
    {
        StopDash();
        playerState = PlayerState.Hiding;
    }

    public void StartMovement()
    {
        playerState = PlayerState.Normal;
    }

    private void OnPlayerHide(HidingSpot spot)
    {
        StopMovement();

        animator.SetBool("IsHiding", true);
    }

    private void OnPlayerExitHiding(HidingSpot spot)
    {
        playerState = PlayerState.Normal;

        animator.SetBool("IsHiding", false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Walls")) StopDash();
    }
}