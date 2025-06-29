using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputReader
{
    // Input Event'leri
    event Action<Vector2> MovesEvent;
    event Action<AttackInput> AttackEvent;
    event Action AttackCanceledEvent;
    event Action RollEvent;
    event Action ParryEvent;
    event Action InteractEvent;
    event Action PreviousEvent;
    event Action DashEvent;
    void OnMovePerformed(InputAction.CallbackContext context);
    void OnMoveCanceled(InputAction.CallbackContext context);
    void OnLightAttackPerformed(InputAction.CallbackContext context);
    void OnAttackCanceled(InputAction.CallbackContext context);
    void OnDashPerformed(InputAction.CallbackContext context);
    void OnRollPerformed(InputAction.CallbackContext context);
    void OnParryPerformed(InputAction.CallbackContext context);
    void OnInteract(InputAction.CallbackContext context);
    void OnPrevious(InputAction.CallbackContext context);
    void BindInput(InputContext currentMap);
    void UnbindAllInput();

}
