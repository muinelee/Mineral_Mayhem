using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.EventSystems;

public class RaesCharacterMovementHandler : NetworkBehaviour
{

    RaesNetworkCharacterControllerPrototype networkCharacterControllerPrototype;
    private void Awake()
    {
        networkCharacterControllerPrototype = GetComponent<RaesNetworkCharacterControllerPrototype>(); 
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public override void FixedUpdateNetwork()
    {
        //get input from the network 
        if (GetInput(out RaesNetworkInputData networkInputData))
        {
            // Moving 
            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();
            Debug.Log("Receiving input"); 
            networkCharacterControllerPrototype.Move(moveDirection); 
        }
    }
}
