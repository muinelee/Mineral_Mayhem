using Cinemachine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class NetworkPlayer_InputController : NetworkBehaviour
{
    private Vector3 moveInputVector;
    private Vector3 cursorLocation;
    private float lookDirection;
    private bool isDashPressed = false;
    private bool isQPressed = false;
    private bool isEPressed = false;
    private bool isFPressed = false;

    [SerializeField] private Camera cam;

    [Networked] public NetworkPlayer NetworkUser { get; set; }

    private void Update()
    {
        if (!Object.HasInputAuthority) return;

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
        // Apply Move Input
        moveInputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Apply Cursor location
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit)) cursorLocation = raycastHit.point;

        // Apply Attack Input
        if (Input.GetKeyDown(KeyCode.Space)) isDashPressed = true;
        if (Input.GetKeyDown(KeyCode.Q)) isQPressed = true;
        if (Input.GetKeyDown(KeyCode.E)) isEPressed = true;
        if (Input.GetKeyDown(KeyCode.F)) isFPressed = true;
    }
}