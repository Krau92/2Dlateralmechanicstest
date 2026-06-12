using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    private PlayerMovement movement;
    private PlayerCombat combat;
    private InputSystem_Actions input;

    public void Initialize(PlayerMovement movement, PlayerCombat combat)
    {
        this.movement = movement;
        this.combat = combat;
    }

    void OnEnable()
    {
        if (input == null)
            input = new InputSystem_Actions();
        input.Enable();
        input.Player.AddCallbacks(this);
    }

    void OnDisable()
    {
        input.Player.RemoveCallbacks(this);
        input.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement?.OnMove(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            movement?.OnJumpPerformed();
        else if (context.canceled)
            movement?.OnJumpCanceled();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && combat != null)
            combat.OnAttack(movement.GetState());
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed && combat != null)
            combat.OnShoot(movement.GetState());
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
            movement?.OnSprint(true);
        else if (context.canceled)
            movement?.OnSprint(false);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
            movement?.OnDash();
    }
}
