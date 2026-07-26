using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum PlayerState
    {
        Normal,
        Hiding
    }
    
    public static event Action OnPlayerDeath;
    
    public KidInput Input => KidInput.Instance;
    
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private HealthSystem healthSystem;

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
    private bool isBeingChased;

    public PlayerState playerState { get; private set; } = PlayerState.Normal;

    public static PlayerMovement instance;

    void Awake()
    {
        instance = this; // make an instance :D
    }

    void OnEnable()
    {
        HidingSpot.OnPlayerHid += OnPlayerHide;
        HidingSpot.OnPlayerExit += OnPlayerExitHiding;

        TransitionManager.TransitionStarted += StopMovement;
        TransitionManager.TransitionEnded += StartMovement;

        healthSystem.OnCharacterDeath += OnDeath;

        ChaseSequence.OnChaseSequenceStart += StopMovementChase;
        ChaseSequence.OnChaseSequenceChase += StartMovementChase;

        Door.OnRoomChanged += Inmunity;
        Door.OnRoomChangedEnd += DisableInmunity;
    }

    void OnDisable()
    {
        HidingSpot.OnPlayerHid -= OnPlayerHide;
        HidingSpot.OnPlayerExit -= OnPlayerExitHiding;

        TransitionManager.TransitionStarted -= StopMovement;
        TransitionManager.TransitionEnded -= StartMovement;

        healthSystem.OnCharacterDeath -= OnDeath;

        ChaseSequence.OnChaseSequenceStart -= StopMovementChase;
        ChaseSequence.OnChaseSequenceChase -= StartMovementChase;

        Door.OnRoomChanged -= Inmunity;
        Door.OnRoomChangedEnd -= DisableInmunity;
    }

    void Start()
    {
        Input.SubscribeToInputAction(Input.MoveAction, null, ChangeMovingDirection, ChangeMovingDirection);
        Input.SubscribeToInputAction(Input.DashAction, StartDash, null, null);
    }

    void OnDestroy()
    {
        Input.UnsubscribeToInputAction(Input.MoveAction, null, ChangeMovingDirection, ChangeMovingDirection);
        Input.UnsubscribeToInputAction(Input.DashAction, StartDash, null, null);
    }

    void FixedUpdate()
    {
        if(playerState != PlayerState.Normal) return;
        
        HandleMove();
        HandleDash();
    }

    private void Inmunity()
    {
        healthSystem.IsInmune = true;
    }

    private void DisableInmunity()
    {
        healthSystem.IsInmune = false;
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
        
        //rb.linearVelocity = dashingDirection * dashVelocity;
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
        //rb.linearVelocity = Vector2.zero;
        isDashing = false;
        
        if(dashCoroutine == null) return;
        StopCoroutine(dashCoroutine);
        dashCoroutine = null;
    }

    #endregion

    private void OnDeath()
    {
        StopMovement();
        animator.SetTrigger("Death");

        OnPlayerDeath?.Invoke();
    }

    public void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("IsMoving", false);
        
        StopDash();
        playerState = PlayerState.Hiding;
    }

    public void StartMovement()
    {
        if(isBeingChased) return;

        playerState = PlayerState.Normal;
    }

    public void StopMovementChase()
    {
        isBeingChased = true;
        StopMovement();
    }

    public void StartMovementChase()
    {
        isBeingChased = false;
        StartMovement();
    }

    private void OnPlayerHide(HidingSpot spot)
    {
        StopMovement();
        Inmunity();

        animator.SetBool("IsHiding", true);
    }

    private void OnPlayerExitHiding(HidingSpot spot)
    {
        playerState = PlayerState.Normal;
        DisableInmunity();

        animator.SetBool("IsHiding", false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Walls")) StopDash();
    }
}