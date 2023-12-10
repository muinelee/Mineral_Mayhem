using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer_Movement : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody networkRigidBody;
    [SerializeField] private float moveSpeed;

    private float turnSmoothVel;
    [SerializeField] [Range(0.1f, 1)] private float turnTime;

    // Start is called before the first frame update
    void Start()
    {
        networkRigidBody = GetComponent<NetworkRigidbody>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            networkRigidBody.Rigidbody.AddForce((Vector3.right * networkInputData.moveDirection.x + Vector3.forward * networkInputData.moveDirection.y) * moveSpeed);
        }
    }

    public void SetLookDirection(Vector3 direction)
    {
        Debug.Log($"Rotating to {direction}");

        // Rotate model in proper direction
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnTime);
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}
