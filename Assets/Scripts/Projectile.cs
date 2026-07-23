using System.Collections;
using UnityEngine;
using uPools;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float projectileDuration = 3;

    void Awake()
    {
        StartCoroutine(ReturnToPool());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        BulletBehaviour(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        BulletBehaviour(collision.gameObject);
    }

    private void BulletBehaviour(GameObject collision)
    {
        if(!collision.TryGetComponent(out HealthSystem healthSystem)) return;

        healthSystem.Damage(damage);

        StopAllCoroutines();
        SharedGameObjectPool.Return(gameObject);
    }

    private IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(projectileDuration);
        SharedGameObjectPool.Return(gameObject);
    }
}