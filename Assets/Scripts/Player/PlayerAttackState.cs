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
        this.skillToPerform = skill; // Hangi yetene�in yap�laca��n� sakla
    }
   
    public void Enter()
    {
        movementController.canMove = false;
        combatController.OnAttack(skillToPerform); // Sald�r�y� ba�lat
        attackTimer = skillToPerform != null ? skillToPerform.skillDuration : 0.3f;
    }


    public void Tick()
    {
        attackTimer -= Time.fixedDeltaTime;
        if (attackTimer <= 0f)
        {
            stateMachine.ChangeState(new PlayerIdleState(stateMachine, movementController, combatController)); // Sald�r� bitince Idle�a d�n
        }
    }

    public void Exit()
    {
        movementController.canMove = true;
        movementController.velocityCut = false;
    }
}
