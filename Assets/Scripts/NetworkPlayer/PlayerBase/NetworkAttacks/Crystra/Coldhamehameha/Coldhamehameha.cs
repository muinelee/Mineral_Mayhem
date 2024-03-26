using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.ProBuilder;
using static Fusion.NetworkCharacterController;

public class Coldhamehameha : NetworkAttack_Base
{
    [Header("Coldhamehameha Properties")]
    [SerializeField] private float beamDuration = 3f;
    [SerializeField] private float startDelay = 2f;
    [SerializeField] private float offset;
    [SerializeField] private float damageOffset;

    [SerializeField] private float radius;
    List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    [SerializeField] private LayerMask collisionLayer;

    private TickTimer blastTimer = TickTimer.None;
    private bool isBeamActive;

    public override void Spawned()
    {
        if (!Object.HasStateAuthority) return;

        transform.position += transform.forward * offset;
        blastTimer = TickTimer.CreateFromSeconds(Runner, beamDuration);
        DealDamage();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        if (blastTimer.Expired(Runner))
        {
            blastTimer = TickTimer.None;
            Runner.Despawn(GetComponent<NetworkObject>());
        }
    }

    void Awake()
    {
        StartCoroutine(ActivateBeamWithDelayCoroutine());
    }

    private IEnumerator ActivateBeamWithDelayCoroutine()
    {
        yield return new WaitForSeconds(startDelay);

        ActivateBeam();
    }

    private void ActivateBeam()
    {
        if (!isBeamActive)
        {
            StartCoroutine(BlastActive());
        }

        
    }

    private IEnumerator BlastActive()
    {
        isBeamActive = true;

        GameObject beamInstance = new GameObject("Coldhamehameha");
        beamInstance.transform.position = transform.position;
        beamInstance.transform.rotation = transform.rotation;

        yield return new WaitForSeconds(beamDuration);

        Destroy(beamInstance);

        isBeamActive = false;
    }

    protected override void DealDamage()
    {
        Vector3 damagePosition = transform.position + transform.forward * damageOffset;

        Runner.LagCompensation.OverlapSphere(damagePosition, radius, player: Object.InputAuthority, hits, collisionLayer, HitOptions.IgnoreInputAuthority);

        for (int i = 0; i < hits.Count; i++)
        {
            Debug.Log($"Did we hit a hitbox? {hits[i].Hitbox}");
            NetworkPlayer_Health healthHandler = hits[i].GameObject.GetComponentInParent<NetworkPlayer_Health>();

            if (healthHandler != null)
            {
                healthHandler.OnTakeDamage(damage);
            }
        }
    }
}
