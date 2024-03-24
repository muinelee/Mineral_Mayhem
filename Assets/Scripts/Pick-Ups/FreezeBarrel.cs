using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FreezeBarrel : NetworkBehaviour
{
    NetworkObject networkObject;
    [SerializeField] private float requiredVelocity = 0.05f;
    [SerializeField] private float freezeDuration = 3f;

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
        // Stopping player's movement after theyre hit
        networkRigidbody.Rigidbody.velocity = Vector3.zero;
        networkRigidbody.Rigidbody.angularVelocity = Vector3.zero;

        StartCoroutine(FreezePlayer(networkRigidbody));
    }

    private IEnumerator FreezePlayer(NetworkRigidbody networkRigidbody)
    {
        // Freezes player 
        networkRigidbody.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        yield return new WaitForSeconds(freezeDuration);
        networkRigidbody.Rigidbody.constraints = RigidbodyConstraints.None;

        Runner.Despawn(networkObject);
    }

}
