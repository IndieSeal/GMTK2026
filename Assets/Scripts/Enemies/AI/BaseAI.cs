using UnityEngine;
using uPools;

public class BaseAI : MonoBehaviour
{
    [System.NonSerialized]
    public SpriteRenderer spriteRenderer;
    public GameObject attackProjectile; // if left null then the enemy will be treated as a melee enemy

    // AI Behaviour
    public float targetDistanceFromPlayer = 10f; // set to 0 for melee enemies

    // Shooting
    float attackCooldown = 0f;
    float currentAttackCooldown = 2f;
    float bulletVelocity = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    public void Update()
    {
        currentAttackCooldown -= Time.deltaTime;
    }

    public bool canShoot()
    {
        if(currentAttackCooldown > 0){ return false; } // on cooldown

        return true;
    }

    public void Shoot(Vector2 targetPosition)
    {
        if(canShoot() == false){ return; }
        currentAttackCooldown = attackCooldown;

        GameObject bulletInstance = SharedGameObjectPool.Rent(attackProjectile, transform.position, Quaternion.identity);
        Vector2 heading = (Vector2)transform.position - targetPosition;

        // uhh so like make this shooting thing work later, i gotta make naivgation shi
        if(bulletInstance.TryGetComponent(out Rigidbody2D rb)){
            rb.linearVelocity = heading.normalized * bulletVelocity;
        }
    }
}
