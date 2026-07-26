using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ChargerEnemyAI : BaseAI
{
    public int attackDamage = 1;
    [SerializeField] bool isCharging = false;
    [SerializeField] bool didHitPlayer = false;
    [SerializeField] GameObject playerGameObject = null;

    public new void Start()
    {
        base.Start();
    }

    public new void Update()
    {
        base.Update();

        updatePlayerPushing();
        Shoot();
    }
    public override void updateNavigation(){ /* Nothing, we dont want to move */ }

    void updatePlayerPushing()
    {
        /*if(playerGameObject == null){ return; }
        // we're dragging the player
        playerGameObject.transform.position = transform.position + ((Vector3)rb.linearVelocity.normalized * 1.5f);
        Rigidbody2D playerRb = playerGameObject.GetComponent<Rigidbody2D>();
        if(playerRb){ playerRb.linearVelocity = Vector2.zero; }
        */
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(isCharging == false){ return; }

        int layer = col.gameObject.layer;
        if(layer == LayerMask.NameToLayer("Walls"))
        {
            transform.position -= (Vector3)rb.linearVelocity.normalized * 1.5f; // move back a smidge to free our player
            updatePlayerPushing();
            rb.linearVelocity = Vector2.zero;

            float cooldownMult = didHitPlayer ? 2 : 1;
            currentAttackCooldown = attackCooldown * cooldownMult;
            isCharging = false;

            //if(didHitPlayer){ playerGameObject.transform.GetComponent<HealthSystem>()?.Damage(attackDamage); }
            didHitPlayer = false;
            playerGameObject = null;
        }
        else if(layer == LayerMask.NameToLayer("Player"))
        {
            if(col.transform.name == "Candle"){ return; } // i dont like to hardcode this but i dont want to hit the candle
            //if(didHitPlayer){ return; } // we already hit the player no need to fuck them over some more
            
            playerGameObject = col.gameObject;
            playerGameObject.transform.GetComponent<HealthSystem>()?.Damage(attackDamage);
            didHitPlayer = true;
        }
        else { Debug.Log(layer); return; } // hit something that we shouldn't stop for
    }

    // override the shoot function
    public void Shoot()
    {
        if(base.canShoot() == false || isCharging){return;}
        StartCoroutine(ShootCoroutine());
    }

    private IEnumerator ShootCoroutine()
    {
        // TODO: Check if the player is dead or hidden before attacking, if they're dead then we keep attacking and get an animation loop

        // shoot
        Vector2 playerPos = (Vector2)PlayerMovement.instance.transform.position;
        Vector2 myPos = (Vector2)transform.position;
        Vector2 chargeHeading = (playerPos - myPos).normalized; // maybe use the A* path so we can get unstuck

        // calculate A* path        
        moveWayPoints.Clear(); // clear it so we force recalculate the A* path
        base.updateNavigation();
        rb.linearVelocity = Vector2.zero;

        // use the A* path to determine where to go, this can help the charger go around corners and not get stuck
        int index = (int)(0.1f * moveWayPoints.Count);
        index = Math.Max(1, index);
        chargeHeading = (playerPos - moveWayPoints[index].position).normalized;

        rb.linearVelocity = chargeHeading * moveSpeed;
        isCharging = true;

        yield return null;
    }
}
