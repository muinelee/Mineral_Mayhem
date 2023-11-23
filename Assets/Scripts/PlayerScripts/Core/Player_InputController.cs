using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_InputController : NetworkBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private CameraFollowPoint camFollowPoint;
    private Animator anim;

    // Scripts
    private Player_Movement playerMovement;
    public Player_AttackController attackController;
    private Vector3 inputDirection;
    private Vector3 lookDirection;

    private enum State { Priming, Idle, Run, Attack, Reacting, Block, Dead, Charge};
    private State currentState;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;

        if (!cam) cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        if (!camFollowPoint) camFollowPoint = gameObject.GetComponentInChildren<CameraFollowPoint>();
        if (!anim) anim = gameObject.GetComponentInChildren<Animator>();
        if (!playerMovement) playerMovement = gameObject.GetComponent<Player_Movement>();
        if (!attackController) attackController = gameObject.GetComponent<Player_AttackController>();
    }

    private void Start()
    {
        currentState = State.Priming;

        CinemachineVirtualCamera cinemachineVC = cam.GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVC.Follow = camFollowPoint.transform;
        cinemachineVC.LookAt = camFollowPoint.transform;
    }

    private void Update() 
    {
        if (currentState != State.Dead)
        {
            Aim();
            GetInput();
            if (playerMovement.GetCanMove()) Move();

            if (currentState != State.Priming) return;

            currentState = State.Idle;
        }
    }

    private void GetInput()
    {
        inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        
        if (Input.GetKeyDown(KeyCode.Space)) Dash();

        if (Input.GetMouseButtonDown(0)) ActivateBasic();

        if (Input.GetKeyDown(KeyCode.Q)) PassAttackInput(ref attackController.qAttack, ref attackController.qAttackTimer);
        if (Input.GetKeyDown(KeyCode.E)) PassAttackInput(ref attackController.eAttack, ref attackController.eAttackTimer);
        // if (Input.GetKeyDown(KeyCode.R)) PassAttackINput(ref attackController.rAttack, ref attackCOntroller.rAttackTimer);       Use when ready

        if (Input.GetMouseButtonDown(1)) StartBlock();
        else if (Input.GetMouseButtonUp(1)) EndBlock();

        if (Input.GetMouseButtonDown(2)) camFollowPoint.ToggleLookAheadCam();
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

    void Dash()
    {
        if (inputDirection != Vector3.zero) playerMovement.ActivateMobilitySkill();
    }

#endregion

#region <----- Attack Functions ----->

    public void ActivateBasic()
    {
        PassAttackInput(ref attackController.basicAttack[attackController.attackCounter % attackController.basicAttack.Length], ref attackController.basicAttackTimer);
    }

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

    void StartBlock()
    {
        if (currentState != State.Block && playerMovement.GetCanMove() && attackController.GetCanAttack())     // Can only block when able to attack
        {
            currentState = State.Block;
            playerMovement.SetAbilitySlow(0.45f);
            currentState = State.Block;
            anim.CrossFade("Block", 0.2f);
        }
    }

    void EndBlock()
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