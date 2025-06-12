using System.Collections.Generic;

[System.Serializable]
public class Stat
{
    public string Name { get; private set; }
    public float BaseValue { get; private set; }

    private List<Modifier> modifiers = new List<Modifier>();

    public Stat(string name, float baseValue)
    {
        Name = name;
        BaseValue = baseValue;
    }

    public void AddModifier(Modifier modifier)
    {
        modifiers.Add(modifier);
    }

    public void RemoveModifier(Modifier modifier)
    {
        modifiers.Remove(modifier);
    }

    public float CalculateFinalValue()
    {
        float finalValue = BaseValue;
        foreach (var mod in modifiers)
        {
            finalValue = mod.Apply(finalValue);
        }
        return finalValue;
    }

    public void Reset()
    {
        modifiers.Clear();
    }
}
