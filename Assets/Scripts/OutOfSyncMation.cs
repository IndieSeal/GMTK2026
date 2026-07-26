using UnityEngine;

public class OutOfSyncMation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    void Awake()
    {
        animator.speed = Random.Range(0.8f, 1.2f);
    }
}