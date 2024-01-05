using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeholder_Ult_Secondary : NetworkBehaviour
{
    [Header("Movement Properties")]
    [SerializeField] private float moveSpeed;
    private NetworkRigidbody networkRigidBody;

    public override void Spawned()
    {
        networkRigidBody = GetComponent<NetworkRigidbody>();
    }

    public override void FixedUpdateNetwork()
    {
        networkRigidBody.Rigidbody.AddForce(transform.forward * moveSpeed);
    }
}
