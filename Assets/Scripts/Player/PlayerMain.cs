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

   

   public void StartRoll()
    {
        stateMachine.ChangeState(new PlayerRollState(stateMachine, movementController, combatController));
    }
    public void StartAttack(ComboData combo, int step)
    {
        // ComboManager'dan gelen bilgiyle doðru SkillData'yý al
        if (step < combo.ComboSkills.Count)
        {
            SkillData skillToExecute = combo.ComboSkills[step];
            // PlayerAttackState'e bu yeteneði yapmasýný söyleyerek state'i deðiþtir.
            stateMachine.ChangeState(new PlayerAttackState(stateMachine, movementController, combatController, skillToExecute));
        }
    }
}
