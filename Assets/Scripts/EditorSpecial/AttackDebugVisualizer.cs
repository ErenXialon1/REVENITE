using UnityEngine;

public class AttackDebugVisualizer : MonoBehaviour
{
    public float duration = 0.2f;
    public HitAreaType areaType;
    public float radius;
    public Vector2 size;

    private float timer;

    private void OnDrawGizmos()
    {
      #if UNITY_EDITOR
        Gizmos.color = new Color(1, 0, 0, 0.4f); // Yarý þeffaf kýrmýzý

        if (areaType == HitAreaType.Circle)
        {
            Gizmos.DrawSphere(transform.position, radius);
        }
        else if (areaType == HitAreaType.Box)
        {
            Gizmos.DrawCube(transform.position, size);
        }
#endif
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
            Destroy(gameObject);
    }
}
