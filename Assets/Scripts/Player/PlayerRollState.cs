using UnityEngine;

public class PlayerRollState : IPlayerState
{
    private PlayerStateMachine stateMachine;
    private PlayerMovementController movementController;
    private CombatController combatController;
    private float rollTimer;

    public PlayerRollState(PlayerStateMachine stateMachine, PlayerMovementController movementController, CombatController combatController)
    {
        this.stateMachine = stateMachine;
        this.movementController = movementController;
        this.combatController = combatController;
    }

    public void Enter()
    {
        movementController.canMove = false;
        rollTimer = combatController.RollDuration;
        combatController.OnRoll(); // Roll başlat
    }

    public void Tick()
    {
        rollTimer -= Time.deltaTime;

        if (rollTimer <= 0)
        {
            stateMachine.ChangeState(new PlayerIdleState(stateMachine, movementController, combatController));//bu "animasyonlar eklendiğinde" sonradan kaldırılacak
            combatController.isRolling = false;//bu "animasyonlar eklendiğinde" sonradan kaldırılacak
        }
    }

    public void Exit()
    {
        movementController.canMove = true;
        combatController.OnRollFinish();
    }
}
