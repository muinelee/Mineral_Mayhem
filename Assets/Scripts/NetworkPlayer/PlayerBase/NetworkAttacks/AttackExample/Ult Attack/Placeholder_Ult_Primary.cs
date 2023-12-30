using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Placeholder_Ult_Primary : NetworkBehaviour
{
    // Fires a cone of secondary attacks in front of 

    [Header("Secondary Properties")]
    [SerializeField] private NetworkObject ultSecondaryPrefab;
    [SerializeField] private float secondaryNum;
    [SerializeField] private float angleOfCone;

    [Header("List of objects hit")]
    [SerializeField] private List<Hitbox> objectsHit;         // Check if object is in the objectsHit list. If not, do damage and add to list 

    private void Start()
    {
        SpawnAttacks();
    }

    public override void Spawned()
    {
        SpawnAttacks();
    }

    private void SpawnAttacks()
    {
        float startOfConeAngle = - (angleOfCone / 2);

        if (secondaryNum == 1) Runner.Spawn(ultSecondaryPrefab, transform.position + Vector3.up, transform.rotation, Object.InputAuthority);

        else
        {
            for (int i = 0; i < secondaryNum; i++)
            {
                float angleOffset = startOfConeAngle + ((angleOfCone / (secondaryNum - 1)) * i);
                NetworkObject spawnedSecondary = Runner.Spawn(ultSecondaryPrefab, transform.position + Vector3.up, Quaternion.Euler(transform.forward + Vector3.up * angleOffset), Object.InputAuthority);
                spawnedSecondary.GetComponent<Placeholder_Ult_Secondary>().SetParent(this);
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
}
