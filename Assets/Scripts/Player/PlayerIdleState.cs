using UnityEngine;

public class PlayerIdleState : IPlayerState
{
    private PlayerStateMachine stateMachine;
    private PlayerMovementController movementController;
    private CombatController combatController;

    public PlayerIdleState(PlayerStateMachine stateMachine, PlayerMovementController movementController, CombatController combatController)
    {
        this.stateMachine = stateMachine;
        this.movementController = movementController;
        this.combatController = combatController;
    }

    public void Enter()
    {
        movementController.canMove = true;
        // Idle animasyonu, reset, vs.
    }

    public void Tick()
    {

    }

    public void Exit()
    {
        // Gerekirse çýkýþ iþlemleri
    }
}
