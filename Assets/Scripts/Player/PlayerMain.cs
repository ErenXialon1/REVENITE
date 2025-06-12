using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    [SerializeField] private PlayerMovementController movementController;
    [SerializeField] private CombatController combatController;
    [SerializeField] private MonoBehaviour inputReaderRef; // InputReader baðlanacak
    private IInputReader inputReader;

    private PlayerStateMachine stateMachine;

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        inputReader = inputReaderRef as IInputReader;
    }

    private void Start()
    {
        var idleState = new PlayerIdleState(stateMachine, movementController, combatController);
        stateMachine.Initialize(idleState);
    }

    private void OnEnable()
    {
        if (inputReader != null)
        {
            inputReader.AttackEvent += OnAttackInput;
        }
    }

    private void OnDisable()
    {
        if (inputReader != null)
        {
            inputReader.AttackEvent -= OnAttackInput;
        }
    }

    private void OnAttackInput()
    {
        stateMachine.ChangeState(new PlayerAttackState(stateMachine, movementController, combatController));
    }
}
