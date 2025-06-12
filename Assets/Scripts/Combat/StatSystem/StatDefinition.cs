using UnityEngine;

[CreateAssetMenu(fileName = "NewStatDefinition", menuName = "Stats/Stat Definition")]
public class StatDefinition : ScriptableObject
{
    public string statName;
    public float defaultValue;
}
