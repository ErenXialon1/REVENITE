using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    [SerializeField] private PlayerMovementController movementController;
    [SerializeField] private CombatController combatController;
    [SerializeField] private MonoBehaviour inputReaderRef; // InputReader ba�lanacak
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

   

   public void StartRoll()
    {
        stateMachine.ChangeState(new PlayerRollState(stateMachine, movementController, combatController));
    }
    public void StartAttack(ComboData combo, int step)
    {
        // ComboManager'dan gelen bilgiyle do�ru SkillData'y� al
        if (step < combo.ComboSkills.Count)
        {
            SkillData skillToExecute = combo.ComboSkills[step];
            // PlayerAttackState'e bu yetene�i yapmas�n� s�yleyerek state'i de�i�tir.
            stateMachine.ChangeState(new PlayerAttackState(stateMachine, movementController, combatController, skillToExecute));
        }
    }
}
