using System;
using System.Collections.Generic;

[Serializable]
public class CharacterStats
{
    public string CharacterName { get; set; } = "Unnamed";
    private Dictionary<string, Stat> stats = new();

    public float CurrentHealth { get; private set; }
    public float MaxHealth => GetStat("VIGOR")?.CalculateFinalValue() * 10f ?? 100f;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    public void Initialize(List<StatDefinition> baseStats, string characterName = "Unnamed")
    {
        CharacterName = characterName;
        foreach (var def in baseStats)
        {
            stats[def.statName] = new Stat(def.statName, def.defaultValue);
        }

        CurrentHealth = MaxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }
    public Dictionary<string, Stat> GetAllStats()
    {
        return stats;
    }
    public void TakeDamage(float amount)
    {
        CurrentHealth = Math.Max(CurrentHealth - amount, 0);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        if (CurrentHealth <= 0)
            OnDeath?.Invoke();
    }

    public void Heal(float amount)
    {
        CurrentHealth = Math.Min(CurrentHealth + amount, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public Stat GetStat(string name)
    {
        stats.TryGetValue(name, out var stat);
        return stat;
    }

    public void ApplyModifier(string statName, Modifier modifier)
    {
        if (stats.TryGetValue(statName, out var stat))
        {
            stat.AddModifier(modifier);
        }
    }

    public void RemoveModifier(string statName, Modifier modifier)
    {
        if (stats.TryGetValue(statName, out var stat))
        {
            stat.RemoveModifier(modifier);
        }
    }
}
