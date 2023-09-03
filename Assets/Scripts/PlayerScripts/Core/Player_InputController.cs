using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_InputController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private Animator anim;
    Test_InputControls controls;

    // Scripts
    Player_Movement movement;
    Player_AttackController attackController;
    Vector3 inputDirection;
    Vector3 lookDirection;

    private enum State { Idle, Run, Attack, Reacting, Dodge, Block };
    private State currentState;

    private void Awake()
    {
        controls = new Test_InputControls();
        controls.Test_Input.Enable();
        controls.Test_Input.Move.performed += ctx => SetMove(ctx);
        controls.Test_Input.Move.canceled += ctx => SetMove(ctx);
        controls.Test_Input.Dash.performed += ctx => Dash(ctx);

        controls.Test_Input.Block.performed += ctx => StartBlock(ctx);
        controls.Test_Input.Block.canceled += ctx => EndBlock(ctx);

        controls.Test_Input.Special_Ability_1.performed += ctx => ActivateQ(ctx);
        controls.Test_Input.Special_Ability_2.performed += ctx => ActivateE(ctx);

        if (!cam) cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        if (!anim) anim = gameObject.GetComponentInChildren<Animator>();
        if (!movement) movement = gameObject.GetComponent<Player_Movement>();
        if (!attackController) attackController = gameObject.GetComponent<Player_AttackController>();

        currentState = State.Idle;
    }

    private void Update() 
    {
        Aim();
        if (movement.GetCanMove()) Move();
    }

    void SetMove(InputAction.CallbackContext ctx)
    {
        inputDirection = ctx.action.ReadValue<Vector3>().normalized;
    }

#region <----- Move and Aim Function ----->

    void Move()
    {
        movement.SetDirection(inputDirection);
        Vector3 perpendicularMovement = new Vector3(lookDirection.z, 0, -lookDirection.x);              // Perpendicular vector in 2D space
        float horizontalMovement = Vector3.Dot(inputDirection, perpendicularMovement);                   // Dot product to get horizontal direction

        if (movement.GetCanMove())
        {
            anim.SetFloat("Zaxis", Vector3.Dot(inputDirection,lookDirection), 0.2f, Time.deltaTime);     // Use Dot product to determine forward move direction relevant to look direction
            anim.SetFloat("Xaxis", horizontalMovement, 0.2f, Time.deltaTime);
        }
    }

    void Aim()
    {
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit)) 
        {
            lookDirection = (raycastHit.point - transform.position).normalized;             
            movement.SetLookDirection(lookDirection);       // Aim to point mouse hits
        }
    }

#endregion

#region <----- Dash Function ----->

    void Dash(InputAction.CallbackContext ctx)
    {
        if (inputDirection != Vector3.zero) movement.ActivateMobilitySkill();
    }

#endregion

#region <----- Attack Functions ----->

    public void ActivateQ(InputAction.CallbackContext ctx)
    {
        PassAttackInput(ref attackController.qAttack, ref attackController.qAttackTimer);
    }

    public void ActivateE(InputAction.CallbackContext ctx)
    {
        PassAttackInput(ref attackController.eAttack, ref attackController.eAttackTimer);
    }

    public void PassAttackInput(ref Attack_Attribute attack, ref float attackTimer)
    {
        if (currentState == State.Idle && attackController.GetCanAttack() && attackTimer > attack.coolDown)
        {
            anim.CrossFade(attack.nameOfAttack, 0.1f);
            attackController.ActivateAttack(attack, ref attackTimer);
        }
    }

#endregion

#region <----- Block Functions ----->

    void StartBlock(InputAction.CallbackContext ctx)
    {
        if (currentState != State.Block && movement.GetCanMove() && attackController.GetCanAttack())     // Can only block when able to attack
        {
            currentState = State.Block;
            movement.SetAbilitySlow(0.45f);
            currentState = State.Block;
            anim.CrossFade("Block", 0.2f);
        }
    }

    void EndBlock(InputAction.CallbackContext ctx)
    {
        movement.ResetAbilitySlow();
        if (currentState == State.Block) Invoke("ResetState", 0.2f);
    }

#endregion



    void ResetState()
    {
        currentState = State.Idle;
        anim.CrossFade("Run", 0.2f);

        movement.MovementEnabled();

        attackController.AttacksEnabled();
    }
}