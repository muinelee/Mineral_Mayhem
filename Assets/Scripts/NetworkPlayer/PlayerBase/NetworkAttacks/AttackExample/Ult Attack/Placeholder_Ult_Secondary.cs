using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeholder_Ult_Secondary : NetworkBehaviour
{
    private float moveSpeed;
    private NetworkRigidbody networkRigidBody;

    public override void Spawned()
    {
        networkRigidBody = GetComponent<NetworkRigidbody>();
    }

    public override void FixedUpdateNetwork()
    {
        networkRigidBody.Rigidbody.AddForce(transform.forward * moveSpeed);
    }

    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }
}
