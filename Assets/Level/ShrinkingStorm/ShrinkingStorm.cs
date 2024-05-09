using System.Collections.Generic;
using UnityEngine;
using Fusion;

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

    [Header("Damage Variables")]
    [SerializeField] private float radius;
    [SerializeField] private LayerMask playerLayer;
    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    // Start is called before the first frame update
    void Start() {
        stormCollider = GetComponent<CapsuleCollider>();
        if (!stormCollider) {
            Debug.LogError("No collider found");
        }
        //subscribe to the event
        //Debug.Log("Subscribed to event");

        RoundManager.resetStorm += ResetStorm;
        RoundManager.startStorm += ShrinkStorm;
    }

    private void OnDestroy()
    {
        RoundManager.resetStorm -= ResetStorm;
        RoundManager.startStorm -= ShrinkStorm;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (isShrinking) {
            //lerp the scale of the object to shrink it
            StormScaleChange();
            if (damageTimer.Expired(Runner)) {
            //for each player in the scene
            foreach (CharacterEntity playerchar in characters) {
                //Debug.Log(playerchar.transform.position);
                    //if the player is not in the collider
                    if (!stormCollider.bounds.Contains(playerchar.transform.position)) {
                        //hurt em
                        //Debug.Log("Player is in the storm");
                        DealDamage();
                        //ManageDamage();
                    }
                }
                damageTimer = TickTimer.None;
                damageTimer = TickTimer.CreateFromSeconds(Runner, damageDelay);
            }
        }
    }

    //private void OnDrawGizmos() {
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, radius);
    //}

    protected override void DealDamage() {
        Runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, playerLayer, HitOptions.IgnoreInputAuthority);
        foreach (LagCompensatedHit hit in hits) {
            IHealthComponent healthComponent = hit.GameObject.GetComponentInParent<IHealthComponent>();

            if (healthComponent != null) {
                if (!stormCollider.bounds.Contains(hit.GameObject.transform.position)) {
                    healthComponent.OnTakeDamage(damage);
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
        damageTimer = TickTimer.CreateFromSeconds(Runner, damageDelay);
        //Debug.Log(isShrinking);
    }
    
    private void StormScaleChange() {
        remainingTime += Time.fixedDeltaTime;
        transform.localScale = Vector3.Lerp(startScale, endScale, (remainingTime / shrinkDuration));
    }

    private void ManageDamage() {
        if (!damageTimer.Expired(Runner)) return;

        damageTimer = TickTimer.None;
        damageTimer = TickTimer.CreateFromSeconds(Runner, damageDelay);
        DealDamage();
    }

    private void ResetStorm(){
        transform.localScale = startScale;
    }
}