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
    public bool canMove = true;
    private Vector3 targetDirection;
    private float dashSpeed = 0;
    private float turnSmoothVel;

    [Header("Dash Properties")]
    [SerializeField] private SO_NetworkDash dash;
    private TickTimer dashCoolDownTimer = TickTimer.None;
    private TickTimer dashDurationTimer = TickTimer.None;
    private bool isDashing = false;

    // Components to be retrieved when Spawned
    [SerializeField] private Animator anim;
    [SerializeField] private NetworkRigidbody networkRigidBody;

    public override void Spawned()
    {
        if (!anim) Debug.Log("Add Animator through inspector");
        if (!networkRigidBody) Debug.Log("Add NetworkRigidbody through inspector");
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            if (canMove && !isDashing && !anim.GetBool("isAttacking"))
            {
                // Set direction player is looking at
                targetDirection = (networkInputData.cursorLocation - transform.position).normalized;

                // Rotate
                Aim();

                // Move
                networkRigidBody.Rigidbody.AddForce(networkInputData.moveDirection * (moveSpeed + dashSpeed));

                // Dash (Can be a boost or buff)
                if (networkInputData.isDashing) MobilityAbility(networkInputData.moveDirection);

                // Play movement animation
                PlayMovementAnimation(networkInputData.moveDirection);
            }

            // Manage Timers and ability effects
            ManageTimers();
        }
    }

    private void Aim()
    {
         // Rotate player using the networkplayer_movement
         float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
         float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnTime);
         transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    private void MobilityAbility(Vector3 moveDirection)
    {
        if (dashCoolDownTimer.IsRunning) return;

        if (!dash.GetCanSteer())
        {
            networkRigidBody.Rigidbody.velocity = moveDirection * dash.GetDashValue();
            isDashing = true;
        }

        else dashSpeed = dash.GetDashValue();

        // Start the timers
        dashCoolDownTimer = TickTimer.CreateFromSeconds(Runner, dash.GetCoolDown());
        dashDurationTimer = TickTimer.CreateFromSeconds(Runner, dash.GetDashDuration());
    }

    private void PlayMovementAnimation(Vector3 moveDirection)
    {
        Vector3 perpendicularMovement = new Vector3(targetDirection.z, 0, -targetDirection.x);              // Perpendicular vector in 2D space
        float horizontalMovement = Vector3.Dot(moveDirection, perpendicularMovement);                       // Dot product to get horizontal direction

        anim.SetFloat("Zaxis", Vector3.Dot(moveDirection, targetDirection), 0.2f, Runner.DeltaTime);     // Use Dot product to determine forward move direction relevant to look direction
        anim.SetFloat("Xaxis", horizontalMovement, 0.2f, Runner.DeltaTime);
    }

    private void ManageTimers()
    {
        if (dashCoolDownTimer.Expired(Runner)) dashCoolDownTimer = TickTimer.None;
        if (dashDurationTimer.Expired(Runner))
        {
            dashSpeed = 0;
            if (!dash.GetCanSteer()) networkRigidBody.Rigidbody.velocity *= 0.2f;
            isDashing = false;
            dashDurationTimer = TickTimer.None;
        }
    }

    public SO_NetworkDash GetDash()
    {
        return dash;
    }

    public ref TickTimer GetDashCoolDownTimer()
    {
        return ref dashCoolDownTimer;
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