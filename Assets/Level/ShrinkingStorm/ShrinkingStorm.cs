using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;
using UnityEngine.VFX;

public class ShrinkingStorm : NetworkAttack_Base { 

    [Header("Shrinking Storm Variables")]
    //timer for start delay
    private TickTimer shrinkTimer = TickTimer.None;
    //var to hold remaining time
    private float remainingTime;
    [SerializeField] private Vector3 startScale;
    [SerializeField] private Vector3 endScale;
    [SerializeField] private float shrinkDuration;

    [Header("Other Variables")]
    //start delay variable
    [SerializeField] private float startDelay;
    [SerializeField] private bool isShrinking = false;
    [SerializeField] private CharacterEntity[] characters;
    private TickTimer damageTimer = TickTimer.None;

    //references
    [Header("References")]
    [SerializeField] private CapsuleCollider stormCollider;
    [SerializeField] private float damageDelay;
    public SplineContainer spline;

    //UI references
    [Header("UI References")]
    [SerializeField] private Image stormImageBase;
    [SerializeField] private Image stormImage2;
    [SerializeField] private Image stormImage3;


    [Header("Damage Variables")]
    [SerializeField] private float radius;
    [SerializeField] private LayerMask playerLayer;
    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    [Header("Visual Effect")]
    [SerializeField] private VisualEffect stormVFX;

    private NetworkRunner runner;

    // Start is called before the first frame update
    void Start() {
        stormCollider = GetComponent<CapsuleCollider>();
        if (!stormCollider) {
            Debug.LogError("No collider found");
        }

        Vector3 spawn = GetSpawnLocation();
        transform.position = spawn;
        Debug.Log("Storm spawned at " + spawn);
        //subscribe to the event
        //Debug.Log("Subscribed to event");

        runner = FindAnyObjectByType<NetworkRunner>();

        if (!runner.IsServer) return;

        RoundManager.resetStorm += ResetStorm;
        RoundManager.startStorm += ShrinkStorm;
    }

    private void OnDestroy()
    {
        if (!runner.IsServer) return;

        RoundManager.resetStorm -= ResetStorm;
        RoundManager.startStorm -= ShrinkStorm;
    }

    // Update is called once per frame
    void FixedUpdate() {

        if (isShrinking) {
            //lerp the scale of the object to shrink it
            StormScaleChange();
            if (damageTimer.Expired(runner)) {
            //for each player in the scene
            foreach (CharacterEntity playerchar in characters) {
                //Debug.Log(playerchar.transform.position);
                    //if the player is not in the collider
                    if (!stormCollider.bounds.Contains(playerchar.transform.position)) {
                        //hurt em
                        //Debug.Log("Player is in the storm");
                        DealDamage();
                        //ManageDamage();
                        //show UI screen for inside storm indicator
                    }
                }
                damageTimer = TickTimer.None;
                damageTimer = TickTimer.CreateFromSeconds(runner, damageDelay);
            }
        }
    }

    //private void OnDrawGizmos() {
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, radius);
    //}

    protected override void DealDamage() {

        if (!runner.IsServer) return;

        runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, playerLayer, HitOptions.IgnoreInputAuthority);
        foreach (LagCompensatedHit hit in hits) {

            // Cores don't have character entities so find objects with a character entities and a health component
            CharacterEntity characterEntity =  hit.GameObject.GetComponentInParent<CharacterEntity>();
            IHealthComponent healthComponent = hit.GameObject.GetComponentInParent<IHealthComponent>();

            if (healthComponent != null && characterEntity != null) {
                if (!stormCollider.bounds.Contains(hit.GameObject.transform.position)) {
                    healthComponent.OnTakeDamage(damage, false);
                    //Debug.Log("player dealt " + damage);
                }
            }
        }
    }

    //private void EventHandler() {
    //    Invoke("ShrinkStorm", startDelay);
    //    Debug.Log("Timer Started");
    //}

    //shrinking the storm over time
    private void ShrinkStorm() {
        isShrinking = true;
        remainingTime = 0f;
        transform.localScale = startScale;
        //list of character positions found
        characters = FindObjectsOfType<CharacterEntity>();
        //set damage timer to 1 second
        damageTimer = TickTimer.CreateFromSeconds(runner, damageDelay);
        //Debug.Log(isShrinking);
    }
    
    private void StormScaleChange() {
        remainingTime += Time.fixedDeltaTime;
        transform.localScale = Vector3.Lerp(startScale, endScale, (remainingTime / shrinkDuration));
    }

    private void ManageDamage() {
        if (!damageTimer.Expired(runner)) return;

        damageTimer = TickTimer.None;
        damageTimer = TickTimer.CreateFromSeconds(runner, damageDelay);
        DealDamage();
    }

    private void ResetStorm(){
        if (!runner.IsServer) return;
        RPC_ResetStorm();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ResetStorm()
    {
        this.transform.localScale = startScale;
        this.ShrinkStorm();
        this.stormVFX.Stop();
        this.stormVFX.Play();

        transform.position = GetSpawnLocation();
    }

    public Vector3 GetSpawnLocation() {
        if (spline != null) {
            float randomLocation = Random.Range(0f, 1f);
            return spline.EvaluatePosition(randomLocation);
        }
        else {
            Debug.LogWarning("Spline is not assigned for core spawning.");
            return Vector3.zero;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, stormCollider.radius * transform.localScale.x);
    }
}