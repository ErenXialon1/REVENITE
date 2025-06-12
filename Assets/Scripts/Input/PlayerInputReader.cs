using System;
using UnityEngine;
using UnityEngine.InputSystem;
public enum InputContext
{
    School,
    Fighting,
    Minigame,
    Cutscene,
}
public class PlayerInputReader : MonoBehaviour, IInputReader
{
    [SerializeField] private InputSystem_Actions inputActions;
    private InputActionMap currentMap;

    // Event tanýmlarý
    public event Action<Vector2> MoveEvent;
    public event Action AttackEvent;
    public event Action AttackCanceledEvent;
    public event Action RollEvent;
    public event Action ParryEvent;
    public event Action InteractEvent;
    public event Action PreviousEvent;



    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();

            SetContext(InputContext.Fighting);
        }

    }
    private void OnDisable()
    {

    }
    public void SetContext(InputContext context)
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
        }

        currentMap.Enable();
        BindInput(context);

    }



    public void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>().normalized;
        MoveEvent?.Invoke(input);
    }

    public void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        MoveEvent?.Invoke(Vector2.zero);
    }

    public void OnAttackPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Attack performed");

        AttackEvent?.Invoke();
    }

    public void OnAttackCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("Attack canceled");
        AttackCanceledEvent?.Invoke();
    }

    public void OnRollPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Roll performed");
        RollEvent?.Invoke();
    }

    public void OnParryPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Parry performed");
        ParryEvent?.Invoke();
    }

    public void OnInteractPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Interact performed");
        InteractEvent?.Invoke();
    }

    public void OnPreviousPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Previous performed");
        PreviousEvent?.Invoke();
    }
    public void UnbindAllInput()
    {
        inputActions.PlayerSchool.Move.performed -= OnMovePerformed;
        inputActions.PlayerSchool.Move.canceled -= OnMoveCanceled;
        inputActions.PlayerSchool.Previous.performed -= OnPreviousPerformed;
        inputActions.PlayerSchool.Interact.performed -= OnInteractPerformed;
        inputActions.PlayerFighting.Move.performed -= OnMovePerformed;
        inputActions.PlayerFighting.Move.canceled -= OnMoveCanceled;
        inputActions.PlayerFighting.Attack.performed -= OnAttackPerformed;
        inputActions.PlayerFighting.Attack.canceled -= OnAttackCanceled;
        inputActions.PlayerFighting.DodgeRoll.performed -= OnRollPerformed;
        inputActions.PlayerFighting.Parry.performed -= OnParryPerformed;
    }

    public void BindInput(InputContext currentMap)
    {
        switch (currentMap)
        {
            case InputContext.School:
                inputActions.PlayerSchool.Move.performed += OnMovePerformed;
                inputActions.PlayerSchool.Move.canceled += OnMoveCanceled;
                inputActions.PlayerSchool.Previous.performed += OnPreviousPerformed;
                inputActions.PlayerSchool.Interact.performed += OnInteractPerformed;
                break;

            case InputContext.Fighting:
                inputActions.PlayerFighting.Move.performed += OnMovePerformed;
                inputActions.PlayerFighting.Move.canceled += OnMoveCanceled;
                inputActions.PlayerFighting.Attack.performed += OnAttackPerformed;
                inputActions.PlayerFighting.Attack.canceled += OnAttackCanceled;
                inputActions.PlayerFighting.DodgeRoll.performed += OnRollPerformed;
                inputActions.PlayerFighting.Parry.performed += OnParryPerformed;

                break;
        }
    }
}
