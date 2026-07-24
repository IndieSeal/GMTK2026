using System;
using UnityEngine;

public class HitReceiver : MonoBehaviour
{
    public event Action<GameObject> OnCollisionHit;
    public event Action<GameObject> OnTriggerHit;
    public event Action<GameObject> OnAnyHit;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionHit?.Invoke(collision.gameObject);
        OnAnyHit?.Invoke(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerHit?.Invoke(collision.gameObject);
        OnAnyHit?.Invoke(collision.gameObject);
    }
}