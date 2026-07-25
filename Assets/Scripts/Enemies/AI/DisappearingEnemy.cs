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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public new void Update()
    {
        base.Update();
        Shoot();
    }


    // just keep the new keyword b/c it doesn't break anything
    public new void Shoot()
    {
        Color spriteColor = base.spriteRenderer.color;
        spriteColor.a = 1; // visible
        base.spriteRenderer.color = spriteColor;
        StartCoroutine(HandleReapearTelegraphDelay()); // wait a sec after reappearing
        base.Shoot( new Vector3(0,0,0) );
        StartCoroutine(HandleReapearTelegraphDelay()); // wait a sec after shooting
        spriteColor.a = 0; // transparent
        base.spriteRenderer.color = spriteColor;
    }
}
