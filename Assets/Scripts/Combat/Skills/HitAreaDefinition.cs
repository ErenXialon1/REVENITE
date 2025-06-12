using UnityEngine;
public enum HitAreaType { Circle, Box }

[CreateAssetMenu(fileName = "New HitArea", menuName = "Combat/Hit Area")]
public class HitAreaDefinition : ScriptableObject
{
    public HitAreaType hitAreaType;
    public Vector2 offset;
    public float radius; // Circle
    public Vector2 size; // Box
}