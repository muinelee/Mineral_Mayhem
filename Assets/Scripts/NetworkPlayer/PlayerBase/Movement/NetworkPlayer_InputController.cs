using Cinemachine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class NetworkPlayer_InputController : CharacterComponent
{
    private Vector3 moveInputVector;
    private Vector3 cursorLocation;
    private float lookDirection;
    private bool isDashPressed = false;
    private bool isQPressed = false;
    private bool isEPressed = false;
    private bool isFPressed = false;
    public bool characterHasBeenSelected = false;

    [SerializeField] private Camera cam;

    [Networked] public NetworkPlayer NetworkUser { get; set; }

    public override void Spawned()
    {
        if (!Object.HasInputAuthority) return;

        CinemachineVirtualCamera virtualCam = GameObject.FindWithTag("PlayerCamera").GetComponent<CinemachineVirtualCamera>();
        virtualCam.Follow = this.transform;
        virtualCam.LookAt = this.transform;
        SetCam(Camera.main);

        FindAnyObjectByType<CharacterSpawner>().SetInputController(this);
    }

    private void Update()
    {
        if (!Object.HasInputAuthority) return;
        if (!characterHasBeenSelected) return;
        GetInput();
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        networkInputData.moveDirection = moveInputVector.normalized;
        networkInputData.cursorLocation = cursorLocation;
        networkInputData.lookDirection = lookDirection;
        networkInputData.isDashing = isDashPressed;

        networkInputData.isQAttack = isQPressed;
        networkInputData.isEAttack = isEPressed;
        networkInputData.isFAttack = isFPressed;

        // reset ability triggers since data has been passed
        isDashPressed = false;
        isQPressed = false;
        isEPressed = false;
        isFPressed = false;

        return networkInputData;
    }

    public void SetCam(Camera camera)
    {
        cam = camera;
    }

    private void GetInput()
    {
        // Apply Cursor location
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit)) cursorLocation = raycastHit.point;

        // Apply Move Input
        moveInputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Apply Attack Input
        if (Input.GetKeyDown(KeyCode.Space)) isDashPressed = true;
        if (Input.GetKeyDown(KeyCode.Q)) isQPressed = true;
        if (Input.GetKeyDown(KeyCode.E)) isEPressed = true;
        if (Input.GetKeyDown(KeyCode.F)) isFPressed = true;
    }
}