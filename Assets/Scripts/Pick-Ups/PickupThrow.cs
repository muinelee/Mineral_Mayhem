using Fusion;
using UnityEngine;

public class PickupThrow : NetworkBehaviour
{
    private NetworkRigidbody rb;
    public float throwForce = 5f;
    public float yStop = 0.1f; //Point pickup stops
    private bool objectStop = false;

    public override void Spawned()
    {
        rb = GetComponent<NetworkRigidbody>();
    }

    public override void FixedUpdateNetwork()
    {
        if (transform.position.y <= yStop && objectStop == false)
        {
            rb.Rigidbody.useGravity = false;
            rb.Rigidbody.velocity = Vector3.zero;
            objectStop = true;
        }
    }

    public void Throw()
    {   
        //Calculate Throw Direction
        rb.Rigidbody.AddForce((transform.forward + Vector3.up) * throwForce, ForceMode.Impulse);   
    }
}
