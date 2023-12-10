using Cinemachine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkPlayer_InputController : NetworkBehaviour
{
    private Vector2 moveInputVector = Vector2.zero;
    private bool isDashPressed = false;
    private bool isQPressed = false;
    private bool isEPressed = false;

    [SerializeField] private NetworkPlayer_Movement playerMovement;
    private Camera cam;
    public CameraFollowPoint camFollowPoint;

    private void Awake()
    {
        playerMovement = GetComponent<NetworkPlayer_Movement>();
        cam = FindAnyObjectByType<Camera>();
    }

    private void Update()
    {
        if (NetworkPlayer.isLocal)
        {
            moveInputVector.x = Input.GetAxis("Horizontal");
            moveInputVector.y = Input.GetAxis("Vertical");

            isDashPressed = Input.GetKeyDown(KeyCode.Space);
            isQPressed = Input.GetKeyDown(KeyCode.Q);
            isEPressed = Input.GetKeyDown(KeyCode.E);

            Aim();
        }
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        networkInputData.moveDirection = moveInputVector.normalized;

        networkInputData.lookDirection = transform.rotation.y;

        networkInputData.isDashing = isDashPressed;
        networkInputData.isQAttack = isQPressed;
        networkInputData.isEAttack = isEPressed;

        return networkInputData;
    }

    private void Aim()
    {
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit))
        {
            Vector3 lookDirection = (raycastHit.point - transform.position).normalized;

            // Move Camera Follow point to proper position
            //camFollowPoint.PassCursorPosition(raycastHit.point);

            // Rotate player using the networkplayer_movement
            playerMovement.SetLookDirection(lookDirection);
        }
   }
}