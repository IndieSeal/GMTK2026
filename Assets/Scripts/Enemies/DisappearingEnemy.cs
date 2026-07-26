using UnityEngine;

public class DisappearingEnemy : EnemyBase
{
    [SerializeField] private BaseAI baseAI;

    public override void OnStartBehaviour()
    {
        base.OnStartBehaviour();

        baseAI.enabled = true;
    }

    public override void OnStopBehaviour()
    {
        base.OnStopBehaviour();

        baseAI.enabled = false;
    }
}