using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Movement), typeof(PlayerDefense))]
public class Test_InputManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    Test_InputControls controls;

    // Scripts
    Movement movement;
    PlayerDefense defense;

    public Vector2 moveDirection;

    private enum State { Idle, Run, Attack, Reacting, Dodge, Block, Dead };
    private State currentState;

    private void Awake()
    {
        controls = new Test_InputControls();
        controls.Test_Input.Enable();
        controls.Test_Input.Move.performed += ctx => Move(ctx);
        controls.Test_Input.Move.canceled += ctx => Move(ctx);
        controls.Test_Input.Dash.performed += ctx => Dash(ctx);

        if (!cam) cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        if (!movement) movement = GetComponent<Movement>();
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
        if (moveDirection != Vector2.zero) movement.ActivateMobilitySkill();
    }
}