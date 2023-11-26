using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer_Movement : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody networkRigidBody;

    // Start is called before the first frame update
    void Start()
    {
        if (!networkRigidBody) networkRigidBody = GetComponent<NetworkRigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {

        }
    }
}
