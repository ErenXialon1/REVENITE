using UnityEngine;

public enum EnemyState
{
    Idle,
    Patrol,
    Chase,
    Attack,
    Dead
}

public abstract class BaseEnemyAI : MonoBehaviour
{
    [SerializeField] protected EnemyState currentState;
    [SerializeField] protected Transform player;
    [SerializeField] protected float speed = 3f;

    protected virtual void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle: Idle(); break;
            case EnemyState.Patrol: Patrol(); break;
            case EnemyState.Chase: Chase(); break;
            case EnemyState.Attack: Attack(); break;
            case EnemyState.Dead: Die(); break;
        }
    }

    protected abstract void Idle();
    protected abstract void Patrol();
    protected abstract void Chase();
    protected abstract void Attack();
    protected abstract void Die();
}
