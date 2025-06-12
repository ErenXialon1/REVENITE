using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputReader
{
    // Input Event'leri
    event Action<Vector2> MoveEvent;
    event Action AttackEvent;
    event Action AttackCanceledEvent;
    event Action RollEvent;
    event Action ParryEvent;
    event Action InteractEvent;
    event Action PreviousEvent;
    void OnMovePerformed(InputAction.CallbackContext context);
    void OnMoveCanceled(InputAction.CallbackContext context);
    void OnAttackPerformed(InputAction.CallbackContext context);
    void OnAttackCanceled(InputAction.CallbackContext context);
    void OnRollPerformed(InputAction.CallbackContext context);
    void OnParryPerformed(InputAction.CallbackContext context);
    void OnInteractPerformed(InputAction.CallbackContext context);
    void OnPreviousPerformed(InputAction.CallbackContext context);
    void BindInput(InputContext currentMap);
    void UnbindAllInput();

}
