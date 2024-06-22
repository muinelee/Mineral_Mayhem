using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PickupSpawner : NetworkBehaviour
{
    public NetworkObject spawnable;
    public Vector3 spawnPoint = Vector3.up * 2f;
    public float respawnTime;

    private NetworkObject currentPickup;
    private Coroutine pickupSpawnCoroutine;

    public override void Spawned()
    {
        if (!Runner.IsServer) return;
        
        StartPickupSpawnTimer(respawnTime);

        if (RoundManager.Instance != null) RoundManager.Instance.ResetRound += PickupDestroy;
    }

    private void StartPickupSpawnTimer(float delay)
    {
        if (pickupSpawnCoroutine != null)
            StopCoroutine(pickupSpawnCoroutine);

        pickupSpawnCoroutine = StartCoroutine(SpawnPickupAfterDelay(delay));
    }

    private IEnumerator SpawnPickupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentPickup = Runner.Spawn(spawnable, transform.position + spawnPoint, Quaternion.identity);

        StartCoroutine(WaitForPickupDestroyed());
    }

    private IEnumerator WaitForPickupDestroyed()
    {
        if (!Runner.IsServer)
        {
            yield return null;
        }
        while (currentPickup != null)
        {
            yield return null;
        }

        StartPickupSpawnTimer(respawnTime);
    }

    public void PickupDestroy()
    {
        if (!Runner.IsServer) return;

        if (currentPickup != null) Runner.Despawn(currentPickup);

        else StartPickupSpawnTimer(respawnTime);
    }
}
