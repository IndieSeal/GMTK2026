using System.Collections.Generic;
using UnityEngine;
using uPools;

public class BaseAI : MonoBehaviour
{
    [System.NonSerialized]
    public SpriteRenderer spriteRenderer;
    [System.NonSerialized]
    public Rigidbody2D rb;
    public GameObject attackProjectile; // if left null then the enemy will be treated as a melee enemy

    // AI Behaviour
    public float moveSpeed = 4f;
    public float targetDistanceFromPlayer = 10f; // set to 0 for melee enemies
    public Vector2 endTargetPosition = Vector2.zero;
    [System.NonSerialized]
    public List<PositionNode> moveWayPoints = new();
    public bool hasReachedAnyWaypoint = true; // if true will recalculate new path

    // Shooting
    public float attackCooldown = 0f;
    [System.NonSerialized]
    public float currentAttackCooldown = 2f;
    public float bulletVelocity = 5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        currentAttackCooldown -= Time.deltaTime;

        updateNavigation();
    }

    // virtual so we can override it in our subclasses
    public virtual void updateNavigation()
    {
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
            moveWayPoints.Add(end); // i dont think `end` is being added in the A* algorithm so imma add it here rq

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
            if(dist > 0.3){ break; }
            moveWayPoints.RemoveAt(0);
            //hasReachedAnyWaypoint = true;
        }

        Vector2 waypointPos = moveWayPoints[0].position;
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

        Vector2 heading = (targetPosition - (Vector2)transform.position).normalized;
        Vector3 spawnPos = transform.position + (Vector3)(heading * 1.4f);
        GameObject bulletInstance = SharedGameObjectPool.Rent(attackProjectile, spawnPos, Quaternion.identity);

        if(bulletInstance.TryGetComponent(out Rigidbody2D rb)){
            rb.linearVelocity = heading * bulletVelocity;
        }
    }
}
