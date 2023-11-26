using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer_InputController : MonoBehaviour
{
    Vector2 moveInputVector = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Movement Direction
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        // Camera Direction
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        networkInputData.moveDirection = moveInputVector;

        return networkInputData;
    }
}
