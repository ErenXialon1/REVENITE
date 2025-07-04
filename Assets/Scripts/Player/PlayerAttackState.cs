using UnityEngine;

public class PlayerAttackState : IPlayerState
{
    private PlayerStateMachine stateMachine;
    private PlayerMovementController movementController;
    private CombatController combatController;

    
    private float attackTimer;
    private SkillData skillToPerform;

    public PlayerAttackState(PlayerStateMachine stateMachine, PlayerMovementController movementController, CombatController combatController, SkillData skill)
    {
        this.stateMachine = stateMachine;
        this.movementController = movementController;
        this.combatController = combatController;
        this.skillToPerform = skill; // Hangi yeteneğin yapılacağını sakla
    }
   
    public void Enter()
    {
        movementController.canMove = false;
        combatController.OnAttack(skillToPerform); // Saldırıyı başlat
        attackTimer = skillToPerform != null ? skillToPerform.skillDuration : 0.3f;
    }


    public void Tick()
    {
        attackTimer -= Time.fixedDeltaTime;
        if (attackTimer <= 0f)
        {
            stateMachine.ChangeState(new PlayerIdleState(stateMachine, movementController, combatController)); //bu "animasyonlar eklendiğinde" sonradan kaldırılacak
        }
    }

    public void Exit()
    {
        movementController.canMove = true;
        movementController.velocityCut = false;
    }
}
