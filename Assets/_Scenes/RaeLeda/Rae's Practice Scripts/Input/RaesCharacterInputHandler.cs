using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaesCharacterInputHandler : MonoBehaviour
{
    Vector2 moveInputVector = Vector2.zero; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveInputVector.x = Input.GetAxis("Horizotnal");
        moveInputVector.y = Input.GetAxis("Vertical"); 
    }

    public RaesNetworkInputData GetNetworkInput()
    {
        RaesNetworkInputData networkInputData = new RaesNetworkInputData();

        networkInputData.movementInput = moveInputVector;

        return networkInputData; 
    }
}
