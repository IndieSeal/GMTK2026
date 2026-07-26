using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ChargerEnemyAI : BaseAI
{
    public int attackDamage = 1;
    bool isCharging = false;

    public new void Start()
    {
        base.Start();
    }

    public new void Update()
    {
        base.Update();
        
        Shoot();
    }

    public override void updateNavigation(){ /* Nothing, we dont want to move */ }

    /*
    void OnTriggerEnter2D()
    {
        /*
        if(col.transform.tag == "Collisions"){}
        else if(col.transform.tag == "Player")
        {
            col.transform.GetComponent<HealthSystem>().Damage(attackDamage);
        }
        else { Debug.Log(transform.tag); return; } // hit something that we shouldn't stop for
        *./

        // on collide
        rb.linearVelocity = Vector2.zero;
        currentAttackCooldown = attackCooldown;
        isCharging = false;

    }
    */
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.transform.name);
    }

    // override the shoot function
    public void Shoot()
    {
        if(base.canShoot() == false || isCharging){return;}
        StartCoroutine(ShootCoroutine());
    }

    private IEnumerator ShootCoroutine()
    {
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
