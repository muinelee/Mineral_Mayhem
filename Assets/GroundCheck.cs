using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public LayerMask groundLayer;
    public bool isGrounded { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (groundLayer == (groundLayer | (1 << other.gameObject.layer)))
        {
            isGrounded = true;
            Debug.Log("Grounded");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (groundLayer == (groundLayer | (1 << other.gameObject.layer)))
        {
            isGrounded = false;
        }
    }
}
