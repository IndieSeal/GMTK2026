using System;
using System.Collections;
using UnityEngine;

public class DisappearingEnemy : BaseAI
{
    public float reappearTelegraphTime = 0.5f;

    private IEnumerator HandleReapearTelegraphDelay()
    {
        yield return new WaitForSeconds(reappearTelegraphTime);
    }


    public new void Start()
    {
        base.Start();
        setTransparency(0);
    }

    public new void Update()
    {
        base.Update();
        Shoot();
    }


    void setTransparency(float alpha)
    {
        Color spriteColor = base.spriteRenderer.color;
        spriteColor.a = alpha;
        base.spriteRenderer.color = spriteColor;
    }

    // just keep the new keyword b/c it doesn't break anything
    public new void Shoot()
    {
        if(base.canShoot() == false){return;}
        StartCoroutine(ShootCoroutine());
    }

    private IEnumerator ShootCoroutine()
    {
        // become visible
        setTransparency(1); // visible
        yield return StartCoroutine(HandleReapearTelegraphDelay()); // wait a sec after reappearing
        
        // shoot
        Vector2 playerPos = (Vector2)PlayerMovement.instance.transform.position;
        Vector2 playerVel = PlayerMovement.instance.GetComponent<Rigidbody2D>().linearVelocity;
        playerVel *= 0; // ignore velocity for right now
        base.Shoot( playerPos + playerVel );
        yield return StartCoroutine(HandleReapearTelegraphDelay()); // wait a sec after shooting
        
        // become invisible again
        setTransparency(0); // transparent
        yield return null;
    }
}
