using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody rb;
    [SerializeField] private CapsuleCollider cc;
    [SerializeField] private Mobility_Attributes ma;    // Scriptable Object from 'Mobility Attributes'                                                       
    [SerializeField] private float movSpd;
    [SerializeField] private float gravityScale;

    private float abilitySlow = 1;
    private float targetAbilitySlow = 1;

    private float statusEffectSlow = 1;

    private bool canMove = true;
    private bool canDash = true;
    private bool dashActive = false;

    private Vector3 direction;
    private Vector3 lookDirection;
    [SerializeField] private float turnTime = 0.1f;
    private float turnSmoothVel;    // Reference variable for DampSmoothAngle

    private float timer = 100f;

    private void Awake()
    {
        if (!rb) rb = gameObject.GetComponent<NetworkRigidbody>();
        if (!cc) cc = gameObject.GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        MobilitySkillTimer();
        if (abilitySlow != targetAbilitySlow) ApplyAbilitySlow();

        rb.Rigidbody.AddForce(Vector3.down * 9.81f * gravityScale, ForceMode.Force);
    }

    public override void FixedUpdateNetwork()
    {
        // ----- Set normal movement ----- //
        if (canMove) Move();
        else if (!canMove && dashActive) rb.Rigidbody.AddForce(direction * movSpd * ma.spdIncrease * Time.deltaTime, ForceMode.Impulse);   // player is using a dash ability
    }

#region <----- Movement and Look function ----->

    private void Move()
    {
        LookAtDirection();
        if (!dashActive) rb.Rigidbody.MovePosition(transform.position + direction * movSpd * abilitySlow * statusEffectSlow * Time.deltaTime);                                     // Normal movement
        else if (dashActive) rb.Rigidbody.MovePosition(transform.position  +  direction * (movSpd + ma.spdIncrease) * abilitySlow * statusEffectSlow * Time.deltaTime);            // Speed buff mobility skills is active
    }

    private void LookAtDirection()
    {
        float targetAngle = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnTime);         // Smooths turning to target angle over time
        transform.rotation = Quaternion.Euler(0, angle, 0);                                                             // Set rotation to angle
    }

#endregion

#region <----- Move direction and look direction variables ----->

    public void SetDirection(Vector3 targetDirection)
    {
        if (canMove) direction = targetDirection;
    }

    public void SetLookDirection(Vector3 targetLookDirection)
    {
        lookDirection = targetLookDirection;
    }

#endregion

#region <----- Mobility Skill functions ----->

    public void ActivateMobilitySkill()
    {
        if (canDash)
        {
            dashActive = true;
            canDash = false;
            timer = 0;
            canMove = ma.canSteer;
            cc.enabled = !ma.canPhase;
            ma.Activate(transform, transform.rotation);
        }
    }

    private void MobilitySkillTimer()
    {
        if (timer < ma.coolDown)    
        {
            timer += Time.deltaTime;
            if (timer > ma.duration) 
            {
                dashActive = false;
                if (!canMove) rb.Rigidbody.velocity = Vector3.zero;
                canMove = true;
                if (!cc.enabled) cc.enabled = true;
            }
        }

        else if (!canDash && canMove) canDash = true;      // Timer is greater than cooldown. player can dash
    }

#endregion

#region <----- Set canMove variables ----->
    
    public void MovementDisabled()
    {
        SetDirection(Vector3.zero);
        canMove = false;
    }

    public void MovementEnabled()
    {
        canMove = true;
    }

    public bool GetCanMove()
    {
        return canMove;
    }

#endregion

#region <----- Ability Slow Functions ----->

    public void SetAbilitySlow(float value)
    {
        if (value < targetAbilitySlow) targetAbilitySlow = value;
    }

    public void ResetAbilitySlow()
    {
        targetAbilitySlow = 1;
    }

    private void ApplyAbilitySlow()
    {
        abilitySlow = Mathf.Lerp(1, targetAbilitySlow, 0.7f);
    }

#endregion

#region <----- Status Effect Slow Functions ----->

    public void SetStatusEffectSlow(float value, float duration)
    {
        if (value < statusEffectSlow) 
        {
            statusEffectSlow = value;
            Invoke("ResetStatusEffectSlow", duration);
        }
    }

    public void ResetStatusEffectSlow()
    {
        statusEffectSlow = 1;
    }

    #endregion

    #region <----- Speed Boost ----->
    public void ApplySpeedBoost(float boostAmount, float boostDuration)
    {
        StartCoroutine(SpeedBoostCoroutine(boostAmount, boostDuration));
    }

    private IEnumerator SpeedBoostCoroutine(float boostAmount, float boostDuration)
    {
        float originalMovSpd = movSpd;

        movSpd += boostAmount;

        yield return new WaitForSeconds(boostDuration);

        movSpd = originalMovSpd;
    }
    #endregion
}