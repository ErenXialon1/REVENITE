using System.Collections;
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
    [SerializeField] private DamageReceiver damageReceiver;
    [SerializeField] private IInputReader inputReader;
    
    [Header("Combat Settings")]
    private Vector2 attackDirection = Vector2.zero;
    private Vector2 lastNonZeroDirection = Vector2.right; // varsayılan olarak aşağıya bakar
    [SerializeField] LayerMask damagableLayer;
    private SkillData currentSkillData;// kullanılan yeteneğe bağlı olarak değişmeli
    public SkillData CurrentSkillData => currentSkillData;
    public ComboData CurrentCombo { get; private set; }
    private bool isAttacking;
    public bool canAttack = true;
    public bool IsAttacking => isAttacking = false;
    
    public bool isParrying = false;
    public bool canParry;
    public bool isParryStance = false;
   

    public event System.Action AttackFinished;
    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 5f; // Dodge mesafesi
    [SerializeField] private float dashDuration = 0.5f; // Dodge süresi
    [SerializeField] private float dashCooldown = 0.5f; // Dodge süresi
    public float isDashing;
    public float canDash;
    [Header("Dodge Roll Settings")]
    [SerializeField] private float rollDistance = 5f; // Dodge mesafesi
    [SerializeField] private float rollDuration = 0.5f; // Dodge süresi
    public float RollDuration => rollDuration;
    [SerializeField] private float rollCooldown = 0.5f; // Cooldown süresi
    public bool isRolling = false;
    public bool IsRolling => isRolling = false;
    public bool canRoll = true;
    [Header("Dodge Roll - Animation Curve")]
    [SerializeField]
    private AnimationCurve rollSpeedCurve = new AnimationCurve(
    new Keyframe(0f, 0f),
    new Keyframe(0.5f, 1f),
    new Keyframe(1f, 0f)
);
    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (characterSkills == null) characterSkills = GetComponent<CharacterSkills>();
        if (playerMovementController == null) playerMovementController = GetComponent<PlayerMovementController>();
        currentSkillData = characterSkills.AvailableSkills[0];//default olarak normal saldırı
        if (characterCombos == null) characterCombos = GetComponent<CharacterCombos>(); 
        if (damageReceiver == null) damageReceiver = GetComponent<DamageReceiver>();
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
        if (!canRoll || isRolling || isParrying || isAttacking) return;
        Debug.Log("save1");
        isRolling = true;
        canRoll = false;
        animator.SetTrigger("Roll");
        // Dodge yönü: Son hareket yönü veya sağa (default)
        Vector2 rollDirection = playerMovementController.LastMoveDirection.normalized;
        if (rollDirection == Vector2.zero) rollDirection = Vector2.right;
        // Hareket uygula (lerp ile smooth hareket için)
        
        StartCoroutine(PerformRollMovement(rollDirection));
        // Burada dodge yönü ve momentum eklenebilir
    }
    private IEnumerator PerformRollMovement(Vector2 direction)
    {
        float elapsed = 0f;
        Vector2 rollVelocity = direction.normalized * (rollDistance / rollDuration);

        while (elapsed < rollDuration) // isRolling flag'i ile kesmeye açık
        {
           /* if (IsRollInterrupted()) // Hasar aldı mı? Başka bir state'e mi geçti? TODO
            {
                rb.linearVelocity = Vector2.zero;
                yield break; // Korutini sonlandır
            }*/

            rb.linearVelocity = rollVelocity * rollSpeedCurve.Evaluate(elapsed / rollDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        //rb.linearVelocity = Vector2.zero;// resetVelocity var
        Debug.Log("save2");
        isRolling = false; //bu "animasyonlar eklendiğinde" sonradan kaldırılacak "onRollFinish"
        canRoll = true;//bu "animasyonlar eklendiğinde"sonradan kaldırılacak "resetRoll"
    }
    public void ExecuteRoll()
    {

    }
    public void ExecuteRollInvincibility()
    {
        damageReceiver.IsInvincible = true;
    }
    public void DisableRollInvincibility()
    {
        damageReceiver.IsInvincible = false;
    }
    public void OnRollFinish()
    {
        isRolling = false;
        Invoke(nameof(ResetRoll), rollCooldown);
    }
    private void ResetRoll()
    {
        canRoll = true;
    }
    public void OnParry()
    {

        if (!canParry || isParrying || isRolling || isAttacking) return;

        isParrying = true;
        canParry = false;
        animator.SetTrigger("Parry");////bu "animasyonlar eklendiğinde" sonradan kaldırılacak bu kısım full animasyon eventler ile dönecek
        Debug.Log("CombatController: Parry triggered.");
        // Parry aktifliği birkaç saniye sürebilir
    }
    
    // Animation Event ile çağrılacak
    public void OnParryStanceStart()
    {
        isParryStance = true;
    }

    // Animation Event ile çağrılacak
    public void OnParryStanceEnd()
    {
        isParryStance = false;
    }

    private void ResetParry() => canParry = true;
}
