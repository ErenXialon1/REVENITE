using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Combat/Skill Data")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public int skillIndex;
    public string skillDescription;
    public SkillEffectType effectType;
    public float baseValue;
    public float impulseStrength;
    public string animationTrigger; // Animasyonun trigger ad�
    public float skillDuration = 0.5f; // Sald�r� s�resi (Idle�a d�n�� s�resi)
    public HitAreaDefinition hitArea;
    public List<ScalingFactor> scalingFactors;
    public List<ModifierDefinition> modifiers;
    public GameObject skillPrefab;
    public AnimationClip skillAnimation;
}

public enum SkillEffectType
{
    Damage,
    Healing
}

[System.Serializable]
public class ScalingFactor
{
    public string statName;
    public float multiplier;
}

[System.Serializable]
public class ModifierDefinition
{
    public float value;
    public ModifierType type;

    public Modifier CreateModifierInstance()
    {
        return type switch
        {
            ModifierType.Flat => new FlatModifier(value),
            ModifierType.Percent => new PercentModifier(value),
            _ => null
        };
    }
}

public enum ModifierType
{
    Flat,
    Percent
}
