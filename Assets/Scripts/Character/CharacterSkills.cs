using UnityEngine;
using System.Collections.Generic;

public class CharacterSkills : MonoBehaviour
{
    [SerializeField] public List<SkillData> availableSkills = new();

    public IReadOnlyList<SkillData> AvailableSkills => availableSkills;

    public void AddSkill(SkillData skill)
    {
        if (!availableSkills.Contains(skill))
        {
            availableSkills.Add(skill);
            Debug.Log($"{skill.skillName} added to {gameObject.name}'s skill list.");
        }
    }

    public void RemoveSkill(SkillData skill)
    {
        if (availableSkills.Contains(skill))
        {
            availableSkills.Remove(skill);
            Debug.Log($"{skill.skillName} removed from {gameObject.name}'s skill list.");
        }
    }

    public bool HasSkill(SkillData skill)
    {
        return availableSkills.Contains(skill);
    }
}
