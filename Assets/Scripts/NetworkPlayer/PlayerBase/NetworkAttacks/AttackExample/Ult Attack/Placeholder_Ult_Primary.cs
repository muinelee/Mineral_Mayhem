using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.Entities.UniversalDelegates;

public class Placeholder_Ult_Primary : NetworkAttack_Base
{
    // Fires a cone of secondary attacks in front of player 

    [Header("Projectile Properties")]
    [SerializeField] private NetworkObject projectilePrefab;
    [SerializeField] private float projectileNum;
    [SerializeField] private float angleOfCone;
    [SerializeField] private float projectileRadius;
    private List<Transform> projectileList = new List<Transform>();

    [Header("List of objects hit")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private List<Hitbox> objectsHit = new List<Hitbox>();         // Check if object is in the objectsHit list. If not, do damage and add to list
    [SerializeField] private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    public override void Spawned()
    {
        if (!Object.HasStateAuthority) return;

        SpawnAttacks();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        DealDamaage();
    }

    private void SpawnAttacks()
    {
        float startOfConeAngle = - (angleOfCone / 2);

        if (projectileNum > 1)
        {
            for (int i = 0; i < projectileNum; i++)
            {
                float angleOffset = startOfConeAngle + ((angleOfCone / (projectileNum - 1)) * i);
                NetworkObject spawnedSecondary = Runner.Spawn(projectilePrefab, transform.position + Vector3.up, transform.rotation * Quaternion.Euler(Vector3.up * angleOffset), Object.InputAuthority);
                projectileList.Add(spawnedSecondary.transform);
            }
        }

        else if (projectileNum == 1) Runner.Spawn(projectilePrefab, transform.position + Vector3.up, transform.rotation, Object.InputAuthority);

        else Debug.Log("This ult does not produce any projectiles");
    }

    private void DealDamaage()
    {
        foreach (Transform projectile in projectileList)
        {
            Runner.LagCompensation.OverlapSphere(projectile.position, projectileRadius, player: Object.InputAuthority, hits, playerLayer, HitOptions.IgnoreInputAuthority);
            foreach (LagCompensatedHit hit in hits)
            {
                if (objectsHit.Contains(hit.Hitbox)) continue;

                objectsHit.Add(hit.Hitbox);
                Debug.Log($"This was hit: {hit.GameObject.name}");

                NetworkPlayer_Health healthHandler = hit.GameObject.GetComponentInParent<NetworkPlayer_Health>();

                if (healthHandler) healthHandler.OnTakeDamage(damage);
            }
        }
    }

    public bool HitboxInObjectsHit(Hitbox objectHit)
    {
        if (objectsHit.Contains(objectHit)) return true;
        else
        {
            objectsHit.Add(objectHit);
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        if (projectileList.Count < 1) return;

        foreach (Transform projectile in projectileList)
        {
            Gizmos.DrawSphere(projectile.position, projectileRadius);
        }
    }
}