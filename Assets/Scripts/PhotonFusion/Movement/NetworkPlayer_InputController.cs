using Cinemachine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkPlayer_InputController : NetworkBehaviour
{
    private Vector2 moveInputVector = Vector2.zero;
    private float lookDirection;
    private bool isDashPressed = false;
    private bool isQPressed = false;
    private bool isEPressed = false;

    private Camera cam;

    private float turnSmoothVel;
    [SerializeField][Range(0.01f, 1f)] private float turnTime;

    private void Update()
    {
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space)) isDashPressed = true;
        if (Input.GetKeyDown(KeyCode.Q)) isQPressed = true;
        if (Input.GetKeyDown(KeyCode.E)) isEPressed = true;

        if (Object.HasInputAuthority) Aim();
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        networkInputData.moveDirection = moveInputVector.normalized;

        networkInputData.lookDirection = lookDirection;

        networkInputData.isDashing = isDashPressed;
        networkInputData.isQAttack = isQPressed;
        networkInputData.isEAttack = isEPressed;

        // reset ability triggers since data has been passed
        isDashPressed = false;
        isQPressed = false;
        isEPressed = false;

        return networkInputData;
    }

    private void Aim()
    {
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit))
        {
            Vector3 targetDirection = (raycastHit.point - transform.position).normalized;

            // Rotate player using the networkplayer_movement
            float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
            lookDirection = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnTime);
        }
    }

    public void SetCam(Camera camera)
    {
        cam = camera;
    }
}