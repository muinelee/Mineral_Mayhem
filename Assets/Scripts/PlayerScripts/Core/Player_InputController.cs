using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_InputController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private CameraFollowPoint camFollowPoint;
    private Animator anim;
    Test_InputControls controls;

    // Scripts
    Player_Movement playerMovement;
    Player_AttackController attackController;
    Vector3 inputDirection;
    Vector3 lookDirection;

    private enum State { Idle, Run, Attack, Reacting, Block, Dead, Charge};
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

        controls.Test_Input.Basic_Attack.performed += ctx => ActivateBasic(ctx);

        controls.Test_Input.Special_Ability_1.performed += ctx => ActivateQ(ctx);
        controls.Test_Input.Special_Ability_1.canceled += ctx => ActivateQCharge(ctx);

        controls.Test_Input.Special_Ability_2.performed += ctx => ActivateE(ctx);
        controls.Test_Input.Special_Ability_2.canceled += ctx => ActivateECharge(ctx);

        controls.Test_Input.Toggle_Look_Ahead_Cam.performed += ctx => camFollowPoint.ToggleLookAheadCam();

        if (!cam) cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        if (!camFollowPoint) camFollowPoint = gameObject.GetComponentInChildren<CameraFollowPoint>();
        if (!anim) anim = gameObject.GetComponentInChildren<Animator>();
        if (!playerMovement) playerMovement = gameObject.GetComponent<Player_Movement>();
        if (!attackController) attackController = gameObject.GetComponent<Player_AttackController>();

        currentState = State.Idle;
    }

    private void Update() 
    {
        if (currentState != State.Dead)
        {
            Aim();
            if (playerMovement.GetCanMove()) Move();
        }
    }

    void SetMove(InputAction.CallbackContext ctx)
    {
        inputDirection = ctx.action.ReadValue<Vector3>().normalized;
    }

#region <----- Move and Aim Function ----->

    void Aim()
    {
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit)) 
        {
            camFollowPoint.PassCursorPosition(raycastHit.point);
            lookDirection = (raycastHit.point - transform.position).normalized;             
            playerMovement.SetLookDirection(lookDirection);       // Aim to point mouse hits
        }
    }

    void Move()
    {
        playerMovement.SetDirection(inputDirection);
        Vector3 perpendicularMovement = new Vector3(lookDirection.z, 0, -lookDirection.x);              // Perpendicular vector in 2D space
        float horizontalMovement = Vector3.Dot(inputDirection, perpendicularMovement);                   // Dot product to get horizontal direction

        if (playerMovement.GetCanMove())
        {
            anim.SetFloat("Zaxis", Vector3.Dot(inputDirection,lookDirection), 0.2f, Time.deltaTime);     // Use Dot product to determine forward move direction relevant to look direction
            anim.SetFloat("Xaxis", horizontalMovement, 0.2f, Time.deltaTime);
        }
    }

#endregion

#region <----- Dash Function ----->

    void Dash(InputAction.CallbackContext ctx)
    {
        if (inputDirection != Vector3.zero) playerMovement.ActivateMobilitySkill();
    }

#endregion

#region <----- Attack Functions ----->

    public void ActivateBasic(InputAction.CallbackContext ctx)
    {
        PassAttackInput(ref attackController.basicAttack[attackController.attackCounter % attackController.basicAttack.Length], ref attackController.basicAttackTimer);
    }

    #region <----- Special Abilities ----->

    public void ActivateQ(InputAction.CallbackContext ctx)
    {
        PassAttackInput(ref attackController.qAttack, ref attackController.qAttackTimer);
    }

    public void ActivateQCharge(InputAction.CallbackContext ctx)
    {
        if (attackController.qAttack.canCharge) PassHoldDuration(ref attackController.qAttack, attackController.qAttackTimer);
    }

    public void ActivateE(InputAction.CallbackContext ctx)
    {
        PassAttackInput(ref attackController.eAttack, ref attackController.eAttackTimer);
    }

    public void ActivateECharge(InputAction.CallbackContext ctx)
    {
        if (attackController.eAttack.canCharge) PassHoldDuration(ref attackController.eAttack, attackController.eAttackTimer);
    }
 
    public void PassHoldDuration(ref Attack_Attribute attack, float attackTimer)
    {
            anim.CrossFade(attackController.qAttack.nameOfAttack + "_Release", 0.2f);
            attackController.PassCharge(attackTimer);
    }

    public void HeldMaxDuration()
    {
        PassHoldDuration(ref attackController.currentAttack, 100);
    }

    #endregion

    public void PassAttackInput(ref Attack_Attribute attack, ref float attackTimer)
    {
        if (currentState == State.Idle && attackController.GetCanAttack() && attackTimer > attack.coolDown)
        {
            anim.CrossFade(attack.nameOfAttack, 0.1f);
            attackController.ActivateAttack(attack, ref attackTimer);
            playerMovement.SetAbilitySlow(1 - attack.attackAbilitySlowPercentage);
            if (attack.stopTurn) playerMovement.MovementDisabled(); 
        }
    }

#endregion

#region <----- Block Functions ----->

    void StartBlock(InputAction.CallbackContext ctx)
    {
        if (currentState != State.Block && playerMovement.GetCanMove() && attackController.GetCanAttack())     // Can only block when able to attack
        {
            currentState = State.Block;
            playerMovement.SetAbilitySlow(0.45f);
            currentState = State.Block;
            anim.CrossFade("Block", 0.2f);
        }
    }

    void EndBlock(InputAction.CallbackContext ctx)
    {
        playerMovement.ResetAbilitySlow();
        if (currentState == State.Block) Invoke("ResetState", 0.2f);
    }

#endregion

    void ResetState()
    {
        currentState = State.Idle;
        anim.CrossFade("Run", 0.2f);

        playerMovement.MovementEnabled();
        playerMovement.ResetAbilitySlow();

        attackController.ResetAttack();
    }

    public void StartDeath()
    {
        playerMovement.SetDirection(Vector3.zero);
        currentState = State.Dead;
        anim.CrossFade("Death", 0.3f);
    }
}