using UnityEngine;

public class SkillPreviewer : MonoBehaviour
{
    public SkillData skillData;
    public Vector2 facingDirection = Vector2.right; // Editor'de yönlendirme amaçlý

    private void OnDrawGizmosSelected()
    {
        if (skillData == null || skillData.hitArea == null)
            return;

        Gizmos.color = Color.red;

        

        if (skillData.hitArea.hitAreaType == HitAreaType.Circle)
        {
            Gizmos.DrawWireSphere((Vector2)transform.position + facingDirection * skillData.hitArea.offset.magnitude, skillData.hitArea.radius);
        }
        else if (skillData.hitArea.hitAreaType == HitAreaType.Box)
        {
            Gizmos.DrawWireCube((Vector2)transform.position + facingDirection * skillData.hitArea.offset.magnitude, skillData.hitArea.size);
        }
    }
}
