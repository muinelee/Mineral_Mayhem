using Cinemachine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer_Movement : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody networkRigidBody;
    [SerializeField] private float moveSpeed;

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
            transform.rotation = Quaternion.Euler(Vector3.up * networkInputData.lookDirection);
        }
    }
}
