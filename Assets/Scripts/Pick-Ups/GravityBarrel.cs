using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GravityBarrel : NetworkBehaviour
{
    NetworkObject networkObject; 
    [SerializeField] private float explosionForce = 1000f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float zeroGravityDuration = 3f;
    [SerializeField] private float damage = 50f;

    private bool exploded = false;

    private void Start()
    {
        networkObject = GetComponent<NetworkObject>(); 
    }

    //Only explodes when velocity upon impact is greater than the required amountt
    private void OnCollisionEnter(Collision collision)
    {
        if (!exploded && collision.gameObject.CompareTag("Player"))
        {
            NetworkRigidbody networkRigidbody = collision.gameObject.GetComponent<NetworkRigidbody>();

            if (networkRigidbody && networkRigidbody.Rigidbody.velocity.magnitude > explosionForce)
            {
                Explode(networkRigidbody);
            }
        } 
    }

    private void Explode(NetworkRigidbody networkRigidbody)
    {
        exploded = true;
        networkRigidbody.Rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius); 
        StartCoroutine(ApplyZeroGravity(networkRigidbody));
    }

    private IEnumerator ApplyZeroGravity(NetworkRigidbody networkRigidbody)
    {
        networkRigidbody.Rigidbody.useGravity = false;

        yield return new WaitForSeconds(zeroGravityDuration);

        networkRigidbody.Rigidbody.useGravity = true; 

        float fallDistance = Mathf.Max(0f, transform.position.y - networkRigidbody.Rigidbody.position.y); 
        float fallDamage = fallDistance * 2f;

        NetworkPlayer_Health playerHealth = networkRigidbody.GetComponent<NetworkPlayer_Health>();
        if (playerHealth)
            playerHealth.OnHit(fallDamage + damage);

        Runner.Despawn(networkObject); 
    }
}
