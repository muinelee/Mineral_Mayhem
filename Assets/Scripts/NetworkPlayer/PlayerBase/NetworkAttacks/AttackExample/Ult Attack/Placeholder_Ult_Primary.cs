using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.Entities.UniversalDelegates;
using Unity.Entities;
using System;
using Unity.VisualScripting;

public class Placeholder_Ult_Primary : NetworkAttack_Base
{
    // Fires a cone of secondary attacks in front of player 

    [Header("End Attack properties")]
    [SerializeField] private float lifetimeDuration;

    private TickTimer timer = TickTimer.None;

    [Header("Projectile Properties")]
    [SerializeField] private NetworkObject projectilePrefab;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileNum;
    [SerializeField] private float angleOfCone;
    [SerializeField] private float projectileRadius;
    private List<NetworkObject> projectileList = new List<NetworkObject>();

    [Header("List of objects hit")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private List<Hitbox> objectsHit = new List<Hitbox>();          // Check if object is in the objectsHit list. If not, do damage and add to list
    [SerializeField] private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    public override void Spawned()
    {
        if (!Object.HasStateAuthority) return;

        SpawnAttacks();                                                             // Create the projectiles

        timer = TickTimer.CreateFromSeconds(Runner, lifetimeDuration);              // Start timer
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        DealDamage();

        ManageTimer();
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
                spawnedSecondary.GetComponent<Placeholder_Ult_Secondary>().SetSpeed(projectileSpeed);
                projectileList.Add(spawnedSecondary);
            }
        }

        else if (projectileNum == 1) Runner.Spawn(projectilePrefab, transform.position + Vector3.up, transform.rotation, Object.InputAuthority);

        else Debug.Log("This ult does not produce any projectiles");
    }

    protected override void DealDamage()
    {
        foreach (NetworkObject projectile in projectileList)
        {
            Runner.LagCompensation.OverlapSphere(projectile.transform.position, projectileRadius, player: Object.InputAuthority, hits, playerLayer, HitOptions.IgnoreInputAuthority);
            foreach (LagCompensatedHit hit in hits)
            {
                if (objectsHit.Contains(hit.Hitbox)) continue;

                objectsHit.Add(hit.Hitbox);
                Debug.Log($"This was hit: {hit.GameObject.name}");

                CharacterEntity characterEntity = hit.GameObject.GetComponentInParent<CharacterEntity>();

                if (characterEntity) characterEntity.OnHit(damage);
                // Temporary solution for between refactoring
                else
                {
                    NetworkPlayer_Health networkPlayer_Health = hit.GameObject.GetComponentInParent<NetworkPlayer_Health>();
                    networkPlayer_Health.OnTakeDamage(damage);

                    StatusHandler statusHandler = hit.GameObject.GetComponentInParent<StatusHandler>();
                    foreach (StatusEffect status in statusEffectSO)
                    {
                        statusHandler.AddStatus(status);
                    }
                }

                if (statusEffectSO.Count > 0 && characterEntity)
                {
                    foreach (StatusEffect status in statusEffectSO)
                    {
                        characterEntity.OnStatusBegin(status);
                    }
                }
            }
        }
    }

    private void ManageTimer()
    {
        if (timer.Expired(Runner))
        {
            foreach (NetworkObject projectile in projectileList) Runner.Despawn(projectile);
            Runner.Despawn(GetComponent<NetworkObject>());
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

    /*
    Gizmos to see projectile area of effect
    
    private void OnDrawGizmos()
    {
        if (projectileList.Count < 1) return;

        foreach (NetworkObject projectile in projectileList)
        {
            Gizmos.DrawWireSphere(projectile.transform.position, projectileRadius);
        }
    }
    */
}