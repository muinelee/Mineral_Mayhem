using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_InputController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    Test_InputControls controls;

    // Scripts
    Player_Movement movement;
    Player_AttackController attackController;

    Vector3 moveDirection;

    private enum State { Idle, Run, Attack, Reacting, Dodge, Block, Dead };
    private State currentState;

    private void Awake()
    {
        controls = new Test_InputControls();
        controls.Test_Input.Enable();
        controls.Test_Input.Move.performed += ctx => Move(ctx);
        controls.Test_Input.Move.canceled += ctx => Move(ctx);
        controls.Test_Input.Dash.performed += ctx => Dash(ctx);
        controls.Test_Input.Special_Ability_1.performed += ctx => ActivateQ(ctx);
        controls.Test_Input.Special_Ability_2.performed += ctx => ActivateE(ctx);

        if (!cam) cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        if (!movement) movement = gameObject.GetComponent<Player_Movement>();
        if (!attackController) attackController = gameObject.GetComponent<Player_AttackController>();
    }

    private void Update() 
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit)) movement.SetLookDirection(raycastHit.point);       // Aim to point mouse hits
    }

    void Move(InputAction.CallbackContext ctx)
    {
        Vector2 input2D = ctx.action.ReadValue<Vector2>();
        moveDirection = Vector3.right * input2D.x + Vector3.forward * input2D.y;                                // Convert 2D input into 3D movement
        movement.SetDirection(moveDirection);
    }

    void Dash(InputAction.CallbackContext ctx)
    {
        if (moveDirection != Vector3.zero) movement.ActivateMobilitySkill();
    }

    void ActivateQ(InputAction.CallbackContext ctx)
    {
        attackController.ActivateAttack(attackController.qAttack, ref attackController.qAttackTimer);
    }

    void ActivateE(InputAction.CallbackContext ctx)
    {
        attackController.ActivateAttack(attackController.eAttack, ref attackController.eAttackTimer);
    }
}