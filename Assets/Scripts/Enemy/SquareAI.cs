using UnityEngine;

public class SquareAI : BaseEnemyAI
{
    protected override void Idle()
    {
        
    }

    protected override void Patrol()
    {
        
        transform.position += Vector3.left * speed * Time.deltaTime;
    }

    protected override void Chase()
    {
        
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    protected override void Attack()
    {
        
        Debug.Log("square is attacking!");
    }

    protected override void Die()
    {
        
        Debug.Log("square died.");
    }
}
