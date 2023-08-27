using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider cc;
    [SerializeField] private Mobility_Attributes ma;    // Scriptable Object from 'Mobility Attributes'                                                       
    [SerializeField] private float movSpd;
    private bool canMove = true;
    private bool canDash = true;
    private bool dashActive = false;

    private Vector3 direction;
    private Vector3 lookDirection;
    private float turnTime = 0.1f;
    private float turnSmoothVel;    // Reference variable for DampSmoothAngle

    private float timer = 100f;

    private void Awake()
    {
        if (!rb) rb = gameObject.GetComponent<Rigidbody>();
        if (!cc) cc = gameObject.GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        MobilitySkillTimer();
    }

    private void FixedUpdate() 
    {
        // ----- Set normal movement ----- //
        if (canMove) Move();

        else if (!canMove && dashActive) rb.velocity = direction * movSpd * ma.spdIncrease * Time.deltaTime;   // player is using a dash ability
    }

    private void Move()
    {
        LookAtDirection(lookDirection);
        if (!dashActive) rb.MovePosition(transform.position + direction * movSpd * Time.deltaTime);                                     // Normal movement
        else if (dashActive) rb.MovePosition(transform.position  +  direction * (movSpd + ma.spdIncrease) * Time.deltaTime);            // Speed buff mobility skills is active
    }

    private void LookAtDirection(Vector3 lookDir)
    {
        float targetAngle = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnTime);         // Smooths turning to target angle over time
        transform.rotation = Quaternion.Euler(0, angle, 0);                                                             // Set rotation to angle
    }

    public void ActivateMobilitySkill()         // Used to link controller to scriptable object. Otherwise it will throw a NullReference error
    {
        if (canDash)
        {
            dashActive = true;
            canDash = false;
            timer = 0;
            canMove = ma.canSteer;
            cc.enabled = !ma.canPhase;
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
                if (!canMove) rb.velocity = Vector3.zero;
                canMove = true;
                if (!cc.enabled) cc.enabled = true;
            }
        }

        else if (!canDash && canMove) canDash = true;      // Timer is greater than cooldown. player can dash
    }

    public void SetDirection(Vector3 targetDirection)
    {
        if (canMove) direction = targetDirection.normalized;
    }

    public void SetLookDirection(Vector3 targetLookDirection)
    {
        lookDirection = (targetLookDirection - transform.position).normalized;
    }

    public void SetCanMove(bool b)
    {
        canMove = b;
    }
}