using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion; 

public class NetworkPickups : NetworkBehaviour
{
    private bool canCollect; 
    void Start()
    {
        
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData) && canCollect)
        {
            
        }

    }

    public void OnPickup()
    {
        // Picking up powerup
        // Applying effect to player 
        // Deleting the powerup for everyone in the scene
    }

    void Update()
    {
        
    }
}
