using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SurgeBarrel : NetworkBehaviour
{
    NetworkObject networkObject; 
    [SerializeField] private float requiredVelocity = 0.05f;
    [SerializeField] private float explosionForce = 1000f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float zeroGravityDuration = 3f;
    [SerializeField] private float damage = 50f;

    public bool exploded = false;

    private void Start()
    {
        networkObject = GetComponent<NetworkObject>(); 
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        //DealDamage();
    }

    //Only explodes when velocity upon impact is greater than the required amountt
    // Surges players backwards to where they're facing 
    private void OnCollisionEnter(Collision collision)
    {
        if (!exploded)
        {
            NetworkRigidbody networkRigidbody = collision.gameObject.GetComponent<NetworkRigidbody>();
            Debug.Log("Collided with player");
            Debug.Log("Velocity:" + networkRigidbody.Rigidbody.velocity.magnitude); 
            if (networkRigidbody && networkRigidbody.Rigidbody.velocity.magnitude > requiredVelocity)
            {
                Explode(networkRigidbody);
            }
        } 
    }


    private void Explode(NetworkRigidbody networkRigidbody)
    {
        exploded = true;
        networkRigidbody.Rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        Vector3 direction = -networkRigidbody.transform.forward;  
        networkRigidbody.Rigidbody.AddForce(direction * explosionForce, 0);
        Runner.Despawn(networkObject); 
    }
}
