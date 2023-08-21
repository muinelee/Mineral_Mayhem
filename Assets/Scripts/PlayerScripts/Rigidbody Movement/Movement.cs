using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] public Rigidbody rb;
    [SerializeField] public Mobility_Attributes ma;    // Scriptable Object from 'Mobility Attributes'                                                                    
    [SerializeField] private float movSpd;
    private bool canMove = true;
    private bool canDash = true;

    public bool mobilitySkillActivated = false;
    private Vector3 direction;
    private Vector3 lookDirection;
    private float turnTime;
    private float turnSmoothVel;    // Reference variable for DampSmoothAngle

    private float timer = 10f;

    private void Awake()
    {
        if (!rb) rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (mobilitySkillActivated)             // Activate mobility skill
        {
            if (ma.canDash)
            {
                ma.Activate();
                timer = 0;
                canMove = ma.canSteer;

                // Space for invincibility
            }

            mobilitySkillActivated = false;
        }

        // ----- Timer ----
        if (timer < ma.coolDown)    
        {
            timer += Time.deltaTime;
            if (timer > ma.duration) 
            {
                if (!canMove) rb.velocity = Vector3.zero;
                canMove = true;
                ma.isActive = false;
            }
        }

        else if (!canDash) canDash = true;
    }

    private void FixedUpdate() 
    {
        // ----- Set normal movement ----- //
        if (canMove)
        {
            LookAtDirection(lookDirection);
            if (!ma.isActive) Move(direction, movSpd);  // Normal movement
            else if (ma.isActive) Move(direction, movSpd + ma.spdIncrease);          // For speed buff type mobility skill
        }

        else if (!canMove && ma.isActive) rb.velocity = direction * movSpd * ma.spdIncrease * Time.deltaTime;   // For Dash type mobility skill
    }

    private void Move(Vector3 dir, float movSpd)
    {
        rb.MovePosition(transform.position + dir * movSpd * Time.deltaTime);
    }

    private void LookAtDirection(Vector3 lookDir)
    {
        float targetAngle = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnTime);         // Smooths turning to target angle over time
        transform.rotation = Quaternion.Euler(0, angle, 0);                                                             // Set rotation to angle
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

    public void ActivateMobilitySkill()     // Used to link controller to scriptable object. Otherwise it will throw a NullReference error
    {
            mobilitySkillActivated = true;
    }

    private void StopMobilitySkill()
    {
        // ----- Decrease SPEED ----- //
        SetCanMove(true);
    }
}