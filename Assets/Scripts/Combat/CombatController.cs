using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerMovementController playerMovementController;
    [SerializeField] private CharacterSkills characterSkills;
    [SerializeField] private CharacterCombos characterCombos;
    [Header("Combat Settings")]
    private Vector2 attackDirection = Vector2.zero;
    private Vector2 lastNonZeroDirection = Vector2.right; // varsayılan olarak aşağıya bakar
    [SerializeField] LayerMask damagableLayer;
    private SkillData currentSkillData;// kullanılan yeteneğe bağlı olarak değişmeli
    public SkillData CurrentSkillData => currentSkillData;
    public ComboData CurrentCombo { get; private set; }

    private bool canAttack = true;
    public bool IsAttacking => isAttacking = false;
    private bool isRolling = false;
    private bool isParrying = false;
    private bool isAttacking;

    public event System.Action AttackFinished;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (characterSkills == null) characterSkills = GetComponent<CharacterSkills>();
        if (playerMovementController == null) playerMovementController = GetComponent<PlayerMovementController>();
        currentSkillData = characterSkills.AvailableSkills[0];//default olarak normal saldırı
        if (characterCombos == null) characterCombos = GetComponent<CharacterCombos>(); 
    }
    public void OnAttack(SkillData skillToPerform)
    {

        if (!canAttack || isRolling || isParrying) return;
        attackDirection = playerMovementController.inputVector;
        SelectSkill(skillToPerform.skillIndex);
        if (currentSkillData == null)
        {
            Debug.LogWarning("CombatController: currentSkillData yok.");
            return;
        }
        // Animasyonu başlat
        animator.SetTrigger(skillToPerform.animationTrigger);
        Debug.Log("CombatController: Attack triggered for combo step " + skillToPerform.skillName);

        // Start cooldown
        canAttack = false;
    }
    // Bu metod animasyonun sonundaki event'ten çağrılacak:
    public void OnAttackFinish()
    {
        isAttacking = false;
        ResetAttack();
        AttackFinished?.Invoke(); // Kombonun bittiğini ComboManager'a haber ver.
    }
    public void SelectSkill(int skillIndex)
    {
        if (characterSkills.availableSkills == null || characterSkills.availableSkills.Count <= skillIndex)
        {
            //Debug.LogWarning("CombatController: Skill skillIndex geçersiz.");
            return;
        }

        currentSkillData = characterSkills.availableSkills[skillIndex];
    }
    public void ApplyAttack(SkillData skillData)
    {
        
        var targets = GetHitTargets(skillData);
        ApplySkillToTargets(skillData, targets);
        SpawnDebugAreaVisual(GetHitAreaCenter(skillData), skillData.hitArea);
    }
   
    private void ApplyImpulse(SkillData skillData)
    {
        rb.AddForce(attackDirection.normalized * skillData.impulseStrength, ForceMode2D.Impulse);
        
    }
    private Collider2D[] GetHitTargets(SkillData skillData)
    {
        HitAreaDefinition hitArea = skillData.hitArea;
        Vector2 center = GetHitAreaCenter(skillData);

        if (hitArea.hitAreaType == HitAreaType.Circle)
        {
            return Physics2D.OverlapCircleAll(center, hitArea.radius, damagableLayer);
        }
        else if (hitArea.hitAreaType == HitAreaType.Box)
        {
            return Physics2D.OverlapBoxAll(center, hitArea.size, 0f, damagableLayer);
        }

        return null;
    }
    private void ApplySkillToTargets(SkillData skillData, Collider2D[] targets)
    {
        var attackerStats = GetComponent<CharacterStatsHandler>();
        if (attackerStats == null) return;

        Skill skill = new Skill(skillData, attackerStats.Stats);

        foreach (var target in targets)
        {
            var receiver = target.GetComponent<DamageReceiver>();
            if (receiver != null)
            {
                receiver.ReceiveDamage(skill);
            }
        }
    }
    public void ExecuteCurrentSkill()
    {
        if (currentSkillData != null)
        {
            ApplyAttack(currentSkillData); // Bölünmüş modüler metodlar
        }
        else
        {
            Debug.LogWarning("No skill data set for this attack.");
        }
    }
    private Vector2 GetHitAreaCenter(SkillData skillData)
    {
        Vector2 usedDirection = attackDirection.normalized != Vector2.zero
        ? attackDirection.normalized
        : playerMovementController.LastMoveDirection;
        
        return (Vector2)transform.position + usedDirection * skillData.hitArea.offset.magnitude;

    }
    void SpawnDebugAreaVisual(Vector2 center, HitAreaDefinition hitArea)
    {

        var debugGO = new GameObject("AttackDebugArea");
        debugGO.transform.position = center;

        var visualizer = debugGO.AddComponent<AttackDebugVisualizer>();
        visualizer.duration = 1f;
        visualizer.areaType = hitArea.hitAreaType;
        visualizer.radius = hitArea.radius;
        visualizer.size = hitArea.size;

    }
    private void ResetAttack()
    {
        canAttack = true;
    }
    public void OnRoll()
    {
        if (isRolling || isParrying || isAttacking) return;

        isRolling = true;
        animator.SetTrigger("Roll");

        // Burada dodge yönü ve momentum eklenebilir
        Invoke(nameof(EndRoll), 0.5f); // örnek süre
    }
    private void EndRoll()
    {
        isRolling = false;
    }
    public void OnParry()
    {

        if (isParrying || isRolling || isAttacking) return;

        isParrying = true;
        animator.SetTrigger("Parry");
        Debug.Log("CombatController: Parry triggered.");
        // Parry aktifliği birkaç saniye sürebilir
        Invoke(nameof(EndParry), 0.3f); // örnek süre
    }
    private void EndParry()
    {
        isParrying = false;
    }
}
