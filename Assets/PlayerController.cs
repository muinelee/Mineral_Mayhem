using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public CinemachineVirtualCamera virtualCamera;
    private Rigidbody rb;
    private PlayerControls controls;
    [SerializeField] private Vector3 moveInput;
    [SerializeField] public GroundCheck groundCheck;


    public int PlayerNumber                                                                 //Property for the player number, so we can separate player information and access it easily
    {
        get { return _playerNumber; }
        set { _playerNumber = value; gameObject.name = "Player " + _playerNumber; }
    }
    private int _playerNumber;

    private enum State { Idle, Run, Attack, Reacting, Dodge, Block, Dead };
    private State currentState;


    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    #region  <------------------------------------------------- Enable/Disable Controls --------------------------------------------------
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    private void FixedUpdate()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move *= speed * Time.deltaTime;
        rb.MovePosition(rb.position + move);
    }

    




}
