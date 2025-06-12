using UnityEngine;

public class PlayerAttackState : IPlayerState
{
    private PlayerStateMachine stateMachine;
    private PlayerMovementController movementController;
    private CombatController combatController;

    
    private float attackTimer;

    public PlayerAttackState(PlayerStateMachine stateMachine, PlayerMovementController movementController, CombatController combatController)
    {
        this.stateMachine = stateMachine;
        this.movementController = movementController;
        this.combatController = combatController;
    }
    public void Enter()
    {
        movementController.canMove = false;
        combatController.OnAttack(); // Saldýrýyý baþlat
        var currentSkill = combatController.CurrentSkillData;
        attackTimer = currentSkill != null ? currentSkill.skillDuration : 0.3f;
    }


    public void Tick()
    {
        attackTimer -= Time.fixedDeltaTime;
        if (attackTimer <= 0f)
        {
            stateMachine.ChangeState(new PlayerIdleState(stateMachine, movementController, combatController)); // Saldýrý bitince Idle’a dön
        }
    }

    public void Exit()
    {
        movementController.canMove = true;
        movementController.velocityCut = false;
    }
}
