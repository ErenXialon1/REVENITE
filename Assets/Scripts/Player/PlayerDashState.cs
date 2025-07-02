using UnityEngine;

public class PlayerDashState : IPlayerState
{
    private PlayerStateMachine stateMachine;
    private PlayerMovementController movementController;
    private CombatController combatController;
    private float dashTimer = 0.5f;

    public PlayerDashState(PlayerStateMachine stateMachine, PlayerMovementController movementController, CombatController combatController)
    {
        this.stateMachine = stateMachine;
        this.movementController = movementController;
        this.combatController = combatController;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Enter()
    {
        combatController.OnDash();
        movementController.canMove = false;
    }

    public void Tick()
    {
        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
            stateMachine.ChangeState(new PlayerIdleState(stateMachine, movementController, combatController));//bu "animasyonlar eklendiðinde" sonradan kaldýrýlacak
        }
    }

    public void Exit()
    {
        movementController.canMove = true;
    }
}
