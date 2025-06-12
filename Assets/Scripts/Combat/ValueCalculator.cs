using UnityEngine;

public class ValueCalculator
{
    private readonly CharacterStats attacker;
    private readonly CharacterStats defender;

    public ValueCalculator(CharacterStats attacker, CharacterStats defender = null)
    {
        this.attacker = attacker;
        this.defender = defender;
    }

    public float CalculateEffect(SkillData skillData)
    {
        float result = skillData.baseValue;

        foreach (var scale in skillData.scalingFactors)
        {
            var stat = attacker.GetStat(scale.statName);
            if (stat != null)
                result += stat.CalculateFinalValue() * scale.multiplier;
        }

        foreach (var modifierDef in skillData.modifiers)
        {
            var modifier = modifierDef.CreateModifierInstance();
            result = modifier.Apply(result);
        }

        // Eðer Damage ise, defender varsa DEF statý düþülür
        if (skillData.effectType == SkillEffectType.Damage && defender != null)
        {
            var defStat = defender.GetStat("DEF");
            float defenseValue = defStat?.CalculateFinalValue() ?? 0f;
            result -= defenseValue * 100f; // sabit multiplier
        }

        return Mathf.Max(result, 0f);
    }
}
