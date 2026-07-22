using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private KidInput input;
    [SerializeField] private Rigidbody2D rb;

    [Header("Moving")]
    [SerializeField] private float movingSpeed = 5;
    private Vector2 currentInput;
    private Vector2 lastInput = Vector2.right;

    [Header("Dashing")]
    [SerializeField] private float dashVelocity = 5;
    [SerializeField] private float dashDuration = 1;
    private Vector2 dashingDirection;
    private bool isDashing;
    private Coroutine dashCoroutine;

    void OnEnable()
    {
        input.SubscribeToInputAction(input.MoveAction, null, ChangeMovingDirection, ChangeMovingDirection);
        input.SubscribeToInputAction(input.DashAction, StartDash, null, null);
    }

    void FixedUpdate()
    {
        HandleMove();
        HandleDash();
    }

    #region Movement

    private void HandleMove()
    {
        if(isDashing) return;

        rb.linearVelocity = (Vector3)currentInput * movingSpeed;
    }

    private void ChangeMovingDirection()
    {
        if(currentInput.sqrMagnitude > 0.01f) lastInput = currentInput.normalized; 
        currentInput = input.MoveAction.ReadValue<Vector2>().normalized;
    }

    #endregion
    #region Dashing

    private void HandleDash()
    {
        if(!isDashing) return;
        
        rb.linearVelocity = dashingDirection * dashVelocity;
    }

    private void StartDash()
    {
        if(isDashing) return;

        dashingDirection = (currentInput.sqrMagnitude > 0.01f) ? currentInput : lastInput;
        dashCoroutine = StartCoroutine(DashCoroutine());
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

    //Make it so it stops dash if you hit a wall, also make it so when it STAYS, as if you dash when you're infront of a wall, nothing happens
}