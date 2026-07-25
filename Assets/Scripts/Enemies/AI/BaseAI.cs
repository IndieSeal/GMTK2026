using UnityEngine;
using uPools;

public class BaseAI : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public GameObject attackProjectile; // if left null then the enemy will be treated as a melee enemy

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

    public void Shoot(Vector3 targetPosition)
    {
        if(canShoot() == false){ return; }
        currentAttackCooldown = attackCooldown;

        GameObject bulletInstance = SharedGameObjectPool.Rent(attackProjectile, transform.position, Quaternion.identity);
        
        if(bulletInstance.TryGetComponent(out Rigidbody2D rb)){
            rb.linearVelocity = Vector2.zero.normalized * bulletVelocity;
        }
    }
}
