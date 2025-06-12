using UnityEngine;

public class Skill
{
    public SkillData skillData;
    private readonly CharacterStats attackerStats;

    public Skill(SkillData data, CharacterStats attackerStats)
    {
        skillData = data;
        this.attackerStats = attackerStats;
    }

    public void Execute(CharacterStats targetStats)
    {
        var valueCalculator = new ValueCalculator(attackerStats, targetStats);
        float effectValue = valueCalculator.CalculateEffect(skillData);

        if (skillData.effectType == SkillEffectType.Damage)
            targetStats.TakeDamage(effectValue);
        else if (skillData.effectType == SkillEffectType.Healing)
            targetStats.Heal(effectValue);

        Debug.Log($"{skillData.skillName} used on {targetStats.CharacterName}, effect: {effectValue}");
    }
}
