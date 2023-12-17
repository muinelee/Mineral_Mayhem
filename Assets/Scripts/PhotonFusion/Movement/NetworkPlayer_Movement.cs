using Cinemachine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer_Movement : NetworkBehaviour
{
    
    [Header("Movement properties")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnTime;
    private float turnSmoothVel;

    private Vector3 targetDirection;
    public bool canMove;

    // Components Set OnSpawn in NetworkPlayer Script
    private Animator anim;
    private NetworkRigidbody networkRigidBody;

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            // Set direction player is looking at
            targetDirection = (networkInputData.cursorLocation - transform.position).normalized;

            // Rotate
            Aim();

            // Move
            networkRigidBody.Rigidbody.AddForce(networkInputData.moveDirection * moveSpeed);

            // Play movement animation
            PlayMovementAnimation(networkInputData.moveDirection);
        }
    }

    private void Aim()
    {
        {
            // Rotate player using the networkplayer_movement
            float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

    private void PlayMovementAnimation(Vector3 moveDirection)
    {
        Vector3 perpendicularMovement = new Vector3(targetDirection.z, 0, -targetDirection.x);              // Perpendicular vector in 2D space
        float horizontalMovement = Vector3.Dot(moveDirection, perpendicularMovement);                       // Dot product to get horizontal direction

        anim.SetFloat("Zaxis", Vector3.Dot(moveDirection, targetDirection), 0.2f, Runner.DeltaTime);     // Use Dot product to determine forward move direction relevant to look direction
        anim.SetFloat("Xaxis", horizontalMovement, 0.2f, Runner.DeltaTime);
    }

    public void SetAnimator(Animator animator)
    {
        anim = animator;
    }

    public void SetNetworkRigidbody(NetworkRigidbody networkRb)
    {
        networkRigidBody = networkRb;
    }
}