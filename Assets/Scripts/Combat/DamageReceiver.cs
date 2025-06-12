using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    [SerializeField]private CharacterStatsHandler statsHandler;

    private void Awake()
    {
        statsHandler = GetComponent<CharacterStatsHandler>();
        if (statsHandler == null)
        {
            statsHandler = GetComponent<CharacterStatsHandler>();
            Debug.LogError($"{gameObject.name} does not have a CharacterStatsHandler. Adding One!");
        }
    }

    public void ReceiveDamage(Skill skill)
    {
        if (skill == null || statsHandler == null)
        {
            Debug.LogWarning("Skill or CharacterStatsHandler is missing.");
            return;
        }

        skill.Execute(statsHandler.Stats);
        Debug.Log(skill.skillData.skillName + "executed");
    }
}
