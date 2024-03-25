using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Physics_ZeroGravity : NetworkAttack_Base
{
    [Header("End Attack Properties")]
    [SerializeField] private float spellDuration;

    private TickTimer spellTimer = TickTimer.None;

    [Header("Spell Properties")]
    [SerializeField] private Vector3 offset;
    [SerializeField] private float disableGravityDuration = 3.0f;
    [SerializeField] private float gravityUpForce = 500.0f;
    [SerializeField] private float gravityDownForce = 500.0f;
    [SerializeField] private float spellRaidus = 3.0f;

    [Header("List of objects hit")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private List<Hitbox> objectsHit = new List<Hitbox>();          // Check if object is in the objectsHit list. If not, do damage and add to list
    [SerializeField] private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    public override void Spawned()
    {
        if (!Object.HasStateAuthority) return;
        spellTimer = TickTimer.CreateFromSeconds(Runner, spellDuration);
        transform.position += transform.up * offset.y + transform.forward * offset.z;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        TurnOffGravity();
        ManageTimer();
    }

    private void TurnOffGravity()
    {
        Vector3 center = this.transform.position;

        Runner.LagCompensation.OverlapSphere(center, spellRaidus, player: Object.InputAuthority, hits, playerLayer, HitOptions.IgnoreInputAuthority);

        foreach (LagCompensatedHit hit in hits)
        {
            if (objectsHit.Contains(hit.Hitbox)) continue;

            objectsHit.Add(hit.Hitbox);
            Debug.Log($"This was hit: {hit.GameObject.name}");

            CharacterEntity characterEntity = hit.GameObject.GetComponentInParent<CharacterEntity>();

            if (statusEffectSO.Count > 0 && characterEntity)
            {
                foreach (StatusEffect status in statusEffectSO)
                {
                    characterEntity.OnStatusBegin(status);
                }
            }

            Rigidbody rb = hit.GameObject.GetComponentInParent<Rigidbody>();

            if (rb)
            {
                StartCoroutine(EnableGravity(rb, characterEntity));
            }
        }
    }

    private IEnumerator EnableGravity(Rigidbody rb, CharacterEntity character)
    {
        rb.useGravity = false;
        rb.AddForce(Vector3.up * gravityUpForce);

        yield return new WaitForSeconds(disableGravityDuration);

        rb.useGravity = true;
        rb.AddForce(Vector3.down * gravityDownForce);
        character.OnHit(damage);
    }

    private void ManageTimer()
    {
        if (spellTimer.Expired(Runner))
        {
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
}
