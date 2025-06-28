using UnityEngine;

public class PlayerParryState : IPlayerState
{

    private PlayerStateMachine stateMachine;
    private PlayerMovementController movementController;
    private CombatController combatController;
    private float parryTimer;
    public PlayerParryState(PlayerStateMachine stateMachine, PlayerMovementController movementController, CombatController combatController)
    {
        this.stateMachine = stateMachine;
        this.movementController = movementController;
        this.combatController = combatController;
    }
    public void Enter()
    {
        movementController.canMove = false;
    }

    public void Tick()
    {
        parryTimer -= Time.deltaTime;

        if (parryTimer <= 0)
        {
            stateMachine.ChangeState(new PlayerIdleState(stateMachine, movementController, combatController));//bu "animasyonlar eklendi�inde" sonradan kald�r�lacak
            combatController.isParrying = false;//bu "animasyonlar eklendi�inde" sonradan kald�r�lacak
        }
    }

    public void Exit()
    {
        movementController.canMove = true;
        combatController.isParrying= false;
    }
}
