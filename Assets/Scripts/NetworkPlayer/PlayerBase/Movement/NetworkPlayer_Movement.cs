using Cinemachine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer_Movement : CharacterComponent
{
    [Header("Movement properties")]
    [SerializeField] private float turnTime;
    private Stat speed;
    public bool canMove = true;
    private Vector3 targetDirection;
    private float dashSpeed = 0;
    private float turnSmoothVel;
    private float currentBoostValue = 0;

    // Variables to apply slow;
    private float turnSlow = 0;
    private float abilitySlow = 1;
    private float statusSlow = 1;

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
        if (!anim) anim = GetComponentInChildren<Animator>();
        if (!networkRigidBody) networkRigidBody = GetComponent<NetworkRigidbody>();
        // Temporary solution for between refactoring
        if (Character) speed = Character.StatusHandler.speed;
        else speed = GetComponent<StatusHandler>().speed;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            if (canMove && !isDashing)
            {
                // Set direction player is looking at
                targetDirection = (networkInputData.cursorLocation - transform.position).normalized;

                // Rotate
                Aim();

                // Move
                networkRigidBody.Rigidbody.AddForce(networkInputData.moveDirection * (GetCombinedSpeed() + dashSpeed) * abilitySlow * statusSlow);

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
        // Rotate player over time to the target angle
        float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnTime + turnSlow);
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = rotation;
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

    public float GetDashCoolDownTimer()
    {
        if (dashCoolDownTimer.IsRunning) return (float)dashCoolDownTimer.RemainingTime(Runner);
        else return 0;
    }

    public void SetTurnSlow(float slowPercentage)
    {
        turnSlow = slowPercentage;
    }

    public void ApplyAbility(SO_NetworkAttack ult)
    {
        SetTurnSlow(ult.GetTurnSlow());
        SetAbilitySlow(ult.GetAbilitySlow());
    }

    public void ResetSlows()
    {
        turnSlow = 0;
        abilitySlow = 1;
    }

    public void ResetTurnSlow()
    {
        turnSlow = 0;
    }

    public void SetAbilitySlow(float slowPercentage)
    {
        Mathf.Clamp(slowPercentage, 0, 1);
        abilitySlow = 1 - slowPercentage;
    }

    public void ResetAbilitySlow()
    {
        abilitySlow = 1;
    }

    public void SetStatusSlow(float slowPercentage)
    {
        Mathf.Clamp(slowPercentage, 0, 1);
        statusSlow = 1 - slowPercentage;
    }

    public void ResetStatusSlow()
    {
        statusSlow = 1;
    }

    public void ApplySpeedBoost(float boostAmount, float boostDuration)
    {
        StartCoroutine(SpeedBoostCoroutine(boostAmount, boostDuration));
    }

    private IEnumerator SpeedBoostCoroutine(float boostAmount, float boostDuration)
    {
        float originalMoveSpeed = 0;

        currentBoostValue += boostAmount;

        yield return new WaitForSeconds(boostDuration);

        currentBoostValue = originalMoveSpeed;
    }

    /// <summary>
    ///Temporary function - to use for getting current speed in between refactoring statuses and abilities
    /// </summary>
    private float GetCombinedSpeed()
    {
        return speed.GetValue() + currentBoostValue;
    }
}