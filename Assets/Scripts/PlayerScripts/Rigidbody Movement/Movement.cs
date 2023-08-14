using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] public Rigidbody rb;
    [SerializeField] private float movSpd;
    [SerializeField] private float turnSpd;
    [SerializeField] private bool canMove;
    private Vector3 direction;
    private Vector3 lookDirection;
    private float turnTime;
    private float turnSmoothVel;                                                                                        // reference variable for DampSmoothAngle


    private void Awake() 
    {
        if (!rb) rb = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate() 
    {
        if (!canMove)
        {
            Move(direction, movSpd);
            LookAtDirection(lookDirection);
        }
    }

    private void Move(Vector3 dir, float movSpd)
    {
        rb.AddForce(dir * movSpd * Time.deltaTime * 100, ForceMode.Force);
    }

    private void LookAtDirection(Vector3 lookDir)
    {
        float targetAngle = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnTime);         // Smooths turning to target angle over time
        transform.rotation = Quaternion.Euler(0, angle, 0);                                                             // Set rotation to angle
    }

    public void SetDirection(Vector3 targetDirection)
    {
        direction = targetDirection;
    }

    public void SetLookDirection(Vector3 targetLookDirection)
    {

        lookDirection = (targetLookDirection - transform.position).normalized;
    }

    public void SetCanMove(bool b)
    {
        canMove = b;
    }

    public void StartDodge(int i)
    {
        if (i == 1) gameObject.layer = LayerMask.NameToLayer("Player - Dodge 1");
        else if (i == 2) gameObject.layer = LayerMask.NameToLayer("Player - Dodge 2");
    }

    public void EndDodge()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
    }
}