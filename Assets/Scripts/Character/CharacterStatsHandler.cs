using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsHandler : MonoBehaviour
{
    [SerializeField] private List<StatDefinition> baseStats;

    public CharacterStats Stats { get; private set; }

    private void Awake()
    {
        Stats = new CharacterStats();
        Stats.Initialize(baseStats);
    }
    
    private void OnEnable()
    {
        Stats.OnDeath += HandleDeath;
        Stats.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        Stats.OnDeath -= HandleDeath;
        Stats.OnHealthChanged -= HandleHealthChanged;
    }

    private void HandleDeath()
    {
        Debug.Log($"{gameObject.name} died.");
    }

    private void HandleHealthChanged(float current, float max)
    {
        Debug.Log($"{gameObject.name} health: {current}/{max}");
    }
}
