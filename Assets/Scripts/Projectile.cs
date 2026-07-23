using System.Collections;
using UnityEngine;
using uPools;

public class Projectile : MonoBehaviour, IPoolCallbackReceiver
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float projectileDuration = 3;
    private bool hasReturnedToPool;

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
        if(hasReturnedToPool) return;
        
        StopAllCoroutines();
        SharedGameObjectPool.Return(gameObject);
        hasReturnedToPool = true;
    }

    private IEnumerator ReturnToPoolCoroutine()
    {
        yield return new WaitForSeconds(projectileDuration);
        SharedGameObjectPool.Return(gameObject);
    }

    public void OnRent()
    {
        StartCoroutine(ReturnToPoolCoroutine());
        hasReturnedToPool = false;
    }

    public void OnReturn(){}
}