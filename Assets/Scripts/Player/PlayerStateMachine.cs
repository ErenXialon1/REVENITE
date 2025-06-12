using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    [SerializeField] private IPlayerState currentState;

    public void Initialize(IPlayerState startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }

    public void ChangeState(IPlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    private void FixedUpdate()
    {
        currentState?.Tick(); // Artýk Tick çaðrýlýyor
    }
}
