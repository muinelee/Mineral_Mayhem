using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Component Variables
    PlayerControls controls;
    Animator anim;
    CharacterController cc;
    GameObject opponent;
    public CinemachineVirtualCamera virtualCamera;

    //Movement Variables
    public Vector3 inputDirection;
    public Vector3 curMoveInput;
    public Vector3 moveDir;
    public Vector3 nextMoveInput;

    //Player Variables
    private float speed = 6f;
    private float gravity = -9.81f;
    private float dodgeDistance = 2f;
    private float invincibleTime = 1f;
    private float rotationSpeed = 10f;
    private float attackRange = 2f;
    private float verticalVelocity;
    private bool isDodging = false;
    private bool justFinishedDodge = false;

    public int PlayerNumber                                                                 //Property for the player number, so we can separate player information and access it easily
    { get  { return _playerNumber; }
      set  { _playerNumber = value; gameObject.name = "Player " + _playerNumber; }
    }
    private int _playerNumber;



    #region <------------------------Game Loop------------------------>
    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Move.performed += ctx => Move(ctx);
        controls.Player.Move.canceled += ctx => Move(ctx);
        controls.Player.Attack.performed += ctx => Attack();
        controls.Player.Dodge.performed += ctx => Dodge();
        controls.Player.Block.performed += ctx => Block(ctx);
        controls.Player.Block.canceled += ctx => Block(ctx);
    }

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        cc = gameObject.GetComponent<CharacterController>();
    }

    private void Update()
    {
        ApplyGravity();                                                                                                                     //We apply the gravity from a separate function for readability
        cc.Move(curMoveInput * speed * Time.deltaTime + Vector3.up * verticalVelocity * Time.deltaTime);                                    //We move the player based on the move direction and the vertical velocity
        RotatePlayer();                                                                                                                     //We rotate the player based on the input direction
        IsMovingCheck();                                                                                                                    //We check if the player is moving for the animator and set the isMoving bool accordingly
    }
    #endregion

    #region<------------------------Controls Toggle------------------------>
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    #endregion

    #region <------------------------Core Movement------------------------>
    void Move(InputAction.CallbackContext ctx)
    {
        Vector2 move = ctx.action.ReadValue<Vector2>();                                     //Gathering the input from the player

        anim.SetFloat("hValue", move.x);                                                    //Setting the movement values for the animator
        anim.SetFloat("vValue", move.y);

        Vector3 camForward = Camera.main.transform.forward;                                 //Setting the direction of movement based on the camera's forward and right vectors
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0f;                                                                  //Setting the y value to 0 so the player doesn't move up or down
        camRight.y = 0f;
        camForward.Normalize();                                                             //Normalizing the vectors so the player doesn't move faster when moving diagonally
        camRight.Normalize();

        inputDirection = camForward * move.y + camRight * move.x;                           //Setting the input direction variable based on the camera's forward and right vectors
        inputDirection.Normalize();                                                         //Normalizing the input direction so the player doesn't move faster when moving diagonally

        if (!isDodging)                                                                     //If the player is not dodging, set the move direction to the input direction
        {
            moveDir = inputDirection;
            curMoveInput = moveDir;
        }
        else                                                                                //If the player is dodging, set the nextMoveInput variable to the input direction, so we don't move during the dodge but still store input from the player during the dodge
        {
            nextMoveInput = inputDirection;
            return;
        }
    }
    void RotatePlayer()
    {
        if (justFinishedDodge)                                                                                                              //using a bool to check if the player just finished a dodge, if so, we don't want to rotate the player during the action
        {
            Quaternion desiredRotation = Quaternion.LookRotation(inputDirection);                                                           //We set the desired rotation to the local variable 'input direction'
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);                     //We rotate the player towards the desired rotation over time using a Spherical Lerping function
            justFinishedDodge = false;                                                                                                      //We set the bool to false so we don't keep rotating the player
            curMoveInput = nextMoveInput;                                                                                                   //We set the move input to the next move input so we can move the player after the dodge normally and wipe the temporary input
            nextMoveInput = Vector3.zero;                                                                                                   //We set the next move input to zero so we don't keep moving the player after the dodge
            return;
        }

        if (isDodging)                                                                                                                      //If the player is dodging, we don't want to rotate the player
        {
            return;
        }   

        if (moveDir.magnitude > 0.01f)                                                                                                      //If the player is moving but not dodging, we rotate the player towards the input direction
        {
            Quaternion desiredRotation = Quaternion.LookRotation(inputDirection);                                                           //We set the desired rotation to the local variable 'desiredRotation'
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);                     //We rotate the player towards the desired rotation over time using a Spherical Lerping function
        }


    }

    void IsMovingCheck()                                                                                                                    //We check if the player is moving for the animator and set the isMoving bool accordingly
    { 
        if (curMoveInput.magnitude > 0)
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }

    void ApplyGravity()                                                                                                                     //We apply the gravity with this function
    {
        if (cc.isGrounded)
        {
            verticalVelocity = 0;                                                                                                           //If the player is grounded, we set the vertical velocity to 0
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;                                                                                   //If the player is not grounded, we apply the gravity to the vertical velocity over time
        }
    }
    #endregion

    #region <------------------------Combat------------------------>
    void Block(InputAction.CallbackContext ctx)
    {
        bool isBlocking = ctx.ReadValueAsButton();
        if (isBlocking)
        {
            anim.SetBool("isBlocking", true);
        }
        else
        {
            anim.SetBool("isBlocking", false);
        }
    }

    IEnumerator DodgeRoll()                                                                                                                 //We use a coroutine for the dodge roll so we can use a timer to control the duration
    {
        isDodging = true;                                                                                                                   //We set the isDodging bool to true so we can't move during the dodge
        float dodgeSpeed = dodgeDistance / invincibleTime;                                                                                  //We calculate the local dodge speed variable based on the dodge distance and the invincible time
        float elapsedTime = 0;                                                                                                              //We create a local variable and set the elapsed time to 0

        anim.SetTrigger("Dodge");                                                                                                           //We trigger the dodge animation
        anim.SetBool("isDodging", true);                                                                                                    //We set the isDodging bool in the animator to true so we can control when to exit the animation

        while (elapsedTime < invincibleTime)                                                                                                //We use a while loop to control the duration of the dodge
        {        
            cc.Move(curMoveInput * dodgeSpeed * Time.deltaTime);                                                                            //We move the player based on the move direction and the dodge speed
            elapsedTime += Time.deltaTime;                                                                                                  //We add the elapsed time to the delta time
            yield return null;                                                                                                              //We return null so we can wait for the next frame
        }

        if (nextMoveInput != Vector3.zero)                                                                                                  //If the player is moving after the dodge, we rotate the player towards the input direction
        {
            Quaternion desiredRotation = Quaternion.LookRotation(nextMoveInput);                                                            //We set the desired rotation to the local variable 'desiredRotation'
            transform.rotation = desiredRotation;                                                                                           //We rotate the player towards the desired rotation
            curMoveInput = nextMoveInput;                                                                                                   //We set the move input to the next move input so we can move the player after the dodge normally and wipe the temporary input
        }
        justFinishedDodge = true;                                                                                                           //We set the bool to true so we can unlock the player movement
        anim.SetBool("isDodging", false);                                                                                                   //We set the isDodging bool in the animator to false so we can exit the animation
        isDodging = false;                                                                                                                  //We set the isDodging bool to false so we can move the player again
    }


    void Dodge()
    { 
        if (isDodging == false)                                                                                                             //If the player is not dodging, we start the coroutine
        {
            StartCoroutine(DodgeRoll());
        }
    }

    void Attack()
    {
        anim.SetTrigger("Attack");
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange))
        {
            if (hit.collider.gameObject.tag == "Enemy")
            {
                opponent = hit.collider.gameObject;
                //enemy.EnemyHit();
            }
        }
    }
    #endregion








}
