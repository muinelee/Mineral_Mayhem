using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ExplosionBarrel : NetworkBehaviour
{
    NetworkObject networkObject;
    [SerializeField] private float requiredVelocity = 0.05f;
    [SerializeField] private float explosionForce = 1000f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float attractionForce = 500f; 
    [SerializeField] private float damage = 50f;

    public bool exploded = false;

    private void Start()
    {
        networkObject = GetComponent<NetworkObject>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
    }

    // Only explodes when velocity upon impact is greater than the required amount
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

        // Applyingg attraction force to nearby objects and if any players are within range they get damaged
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider col in colliders)
        {
            if (col.GetComponent<Rigidbody>() != null && col.gameObject != gameObject)
            {
                Vector3 direction = (transform.position - col.transform.position).normalized;
                col.GetComponent<Rigidbody>().AddForce(direction * attractionForce);
            }
            NetworkPlayer_Health playerHealth = col.GetComponent<NetworkPlayer_Health>();
            if (playerHealth)
                playerHealth.OnHit(damage);
        }
        Runner.Despawn(networkObject);
    }
}
