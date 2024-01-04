using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

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
    [SerializeField] private List<Hitbox> objectsHit;         // Check if object is in the objectsHit list. If not, do damage and add to list 

    public override void Spawned()
    {
        if (!Object.HasStateAuthority) return;

        SpawnAttacks();
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