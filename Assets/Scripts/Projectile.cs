using System.Collections;
using UnityEngine;
using uPools;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float projectileDuration = 3;

    void Awake()
    {
        StartCoroutine(ReturnToPoolCoroutine());
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
        if(DetectWall(collision) || !collision.TryGetComponent(out HitReceiver hitReceiver)) return;

        hitReceiver.HealthSystem.Damage(damage);
        ReturnToPool();
    }

    private bool DetectWall(GameObject collision)
    {
        if (collision.layer == LayerMask.NameToLayer("Walls"))
        {
            ReturnToPool();
            return true;        
        }
        
        return false;
    }

    private void ReturnToPool()
    {
        StopAllCoroutines();
        SharedGameObjectPool.Return(gameObject);
    }

    private IEnumerator ReturnToPoolCoroutine()
    {
        yield return new WaitForSeconds(projectileDuration);
        SharedGameObjectPool.Return(gameObject);
    }
}