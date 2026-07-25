using System;
using System.Collections.Generic;
using UnityEngine;

public class HitReceiver : MonoBehaviour
{
    public event Action<GameObject> OnCollisionHit;
    public event Action<GameObject> OnTriggerHit;
    public event Action<GameObject> OnAnyHit;
    
    private List<GameObject> collisions = new List<GameObject>();
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionHit?.Invoke(collision.gameObject);
        OnAnyHit?.Invoke(collision.gameObject);

        if(!collisions.Contains(collision.gameObject)) collisions.Add(collision.gameObject); 
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (!collisions.Contains(collision.gameObject))
        {
            OnCollisionHit?.Invoke(collision.gameObject);
            OnAnyHit?.Invoke(collision.gameObject);

            collisions.Add(collision.gameObject); 
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerHit?.Invoke(collision.gameObject);
        OnAnyHit?.Invoke(collision.gameObject);

        collisions.Remove(collision.gameObject);
    }
}