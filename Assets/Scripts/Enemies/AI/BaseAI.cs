using System.Collections.Generic;
using UnityEngine;
using uPools;

public class BaseAI : MonoBehaviour
{
    [System.NonSerialized]
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public GameObject attackProjectile; // if left null then the enemy will be treated as a melee enemy

    // AI Behaviour
    public float moveSpeed = 4f;
    public float targetDistanceFromPlayer = 10f; // set to 0 for melee enemies
    public Vector2 endTargetPosition = Vector2.zero;
    public List<PositionNode> moveWayPoints = new();
    public bool hasReachedAnyWaypoint = true; // if true will recalculate new path

    // Shooting
    float attackCooldown = 0f;
    float currentAttackCooldown = 2f;
    float bulletVelocity = 5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    public void Update()
    {
        currentAttackCooldown -= Time.deltaTime;

        // update the target pos. Try to get a set amount of distance away from the player
        Vector2 playerPos = (Vector2)PlayerMovement.instance.transform.position;
        Vector2 enemyPos = (Vector2)transform.position;
        Vector2 headingPlayerToEnemy = (enemyPos - playerPos).normalized;
        endTargetPosition = playerPos + (headingPlayerToEnemy * targetDistanceFromPlayer); // distance from the player, and closest to the enemy's position

        if (hasReachedAnyWaypoint || moveWayPoints.Count == 0)
        {
            // update the navigation
            PositionNode start = AStarPathfinder.instance.getNearestNodeFromPos(enemyPos);
            PositionNode end = AStarPathfinder.instance.getNearestNodeFromPos(endTargetPosition);
            moveWayPoints = AStarPathfinder.instance.generatePath(start, end);

            hasReachedAnyWaypoint = false;
        }

        while (true)
        {
            if(moveWayPoints.Count == 0){
                hasReachedAnyWaypoint = true;
                rb.linearVelocity = Vector2.zero;
                return;
            }

            float dist = Vector2.Distance(enemyPos, moveWayPoints[0].position);
            if(dist > 1){ break; }
            moveWayPoints.RemoveAt(0);
            //hasReachedAnyWaypoint = true;
        }

        Vector2 waypointPos = moveWayPoints[0].position;
        //waypointPos += Vector2.up * 1f; // add `Vector2.up` to the waypoint pos to maybe not let it sink down for no reason?
        rb.linearVelocity = (waypointPos - enemyPos).normalized * moveSpeed;
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
