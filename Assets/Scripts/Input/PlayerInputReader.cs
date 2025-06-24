using System;
using UnityEngine;
using UnityEngine.InputSystem;
public enum InputContext
{
    School,
    Fighting,
    Minigame,
    Cutscene,
    UI,
}
public class PlayerInputReader : MonoBehaviour, IInputReader
{
    [SerializeField] private InputSystem_Actions inputActions;
    private InputActionMap currentMap;

    // Event tanýmlarý
    public event Action<Vector2> MovesEvent;
    public event Action<AttackInput> AttackEvent;
    public event Action AttackCanceledEvent;
    public event Action RollEvent;
    public event Action ParryEvent;
    public event Action InteractEvent;
    public event Action PreviousEvent;



    private void Awake() // Awake kullanmak, OnEnable'dan önce baþlatma garantisi için daha güvenlidir.
    {
        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();
        }
        SetInputContext(InputContext.School);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetInputContext(InputContext.Fighting);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            SetInputContext(InputContext.School);
        }
    }

    public void SetInputContext(InputContext context)
    {
        // Disable the current map first
        currentMap?.Disable();
        UnbindAllInput();
        switch (context)
        {
            case InputContext.School:
                
                currentMap = inputActions.PlayerSchool;
                break;
            case InputContext.Fighting:
                currentMap = inputActions.PlayerFighting;
                break;
            case InputContext.Minigame:
                currentMap = inputActions.PlayerMinigame;
                break;
            case InputContext.Cutscene:
                currentMap = inputActions.PlayerCutscene;
                break;
            case InputContext.UI:
                currentMap = inputActions.UI;
                break;
        }

        currentMap.Enable();
        BindInput(context);

    }
    public void GoBackToSchoolContext()
    {
        
        SetInputContext(InputContext.School);
        
    }
    public void GoUIContext()
    {
        SetInputContext(InputContext.UI);
    }


    public void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>().normalized;
        MovesEvent?.Invoke(input);
    }

    public void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        MovesEvent?.Invoke(Vector2.zero);
    }

    public void OnLightAttackPerformed(InputAction.CallbackContext context)
    {
        //Debug.Log("Light Attack performed");

        AttackEvent?.Invoke(AttackInput.LightAttack);
    }

    public void OnAttackCanceled(InputAction.CallbackContext context)
    {
        //Debug.Log("Attack canceled");
        AttackCanceledEvent?.Invoke();
    }

    public void OnRollPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Roll performed");
        RollEvent?.Invoke();
    }

    public void OnParryPerformed(InputAction.CallbackContext context)
    {
        //Debug.Log("Parry performed");
        ParryEvent?.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interact performed");
        InteractEvent?.Invoke();
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
       Debug.Log("Previous performed");
        PreviousEvent?.Invoke();
    }
    public void UnbindAllInput()
    {
        inputActions.PlayerSchool.Moves.performed -= OnMovePerformed;
        inputActions.PlayerSchool.Moves.canceled -= OnMoveCanceled;
        inputActions.PlayerSchool.Interact.performed -= OnPrevious;
        inputActions.PlayerSchool.Previous.performed -= OnInteract;
        inputActions.PlayerFighting.Moves.performed -= OnMovePerformed;
        inputActions.PlayerFighting.Moves.canceled -= OnMoveCanceled;
        inputActions.PlayerFighting.Attack.performed -= OnLightAttackPerformed;
        inputActions.PlayerFighting.Attack.canceled -= OnAttackCanceled;
        inputActions.PlayerFighting.DodgeRoll.performed -= OnRollPerformed;
        inputActions.PlayerFighting.Parry.performed -= OnParryPerformed;
    }

    public void BindInput(InputContext currentMap)
    {
        switch (currentMap)
        {
            case InputContext.School:
                inputActions.PlayerSchool.Moves.performed += OnMovePerformed;
                inputActions.PlayerSchool.Moves.canceled += OnMoveCanceled;
                inputActions.PlayerSchool.Previous.performed += OnPrevious;
                inputActions.PlayerSchool.Interact.performed += OnInteract;
                break;

            case InputContext.Fighting:
                inputActions.PlayerFighting.Moves.performed += OnMovePerformed;
                inputActions.PlayerFighting.Moves.canceled += OnMoveCanceled;
                inputActions.PlayerFighting.Attack.performed += OnLightAttackPerformed;
                inputActions.PlayerFighting.Attack.canceled += OnAttackCanceled;
                inputActions.PlayerFighting.DodgeRoll.performed += OnRollPerformed;
                inputActions.PlayerFighting.Parry.performed += OnParryPerformed;

                break;
        }
    }
}
