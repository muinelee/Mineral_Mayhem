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
        moveDirection = ctx.action.ReadValue<Vector3>();
        movement.SetDirection(moveDirection);
    }

    void Dash(InputAction.CallbackContext ctx)
    {
        if (moveDirection != Vector3.zero) movement.ActivateMobilitySkill();
    }
}