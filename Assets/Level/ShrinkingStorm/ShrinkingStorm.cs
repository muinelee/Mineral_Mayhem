using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using static Fusion.NetworkCharacterController;
using UnityEngine.TextCore.Text;

public class ShrinkingStorm : NetworkAttack_Base { 

    [Header("Shrinking Storm Variables")]
    //timer for start delay
    private TickTimer shrinkTimer = TickTimer.None;
    //var to hold remaining time
    private float remainingTime;

    [Header("Other Variables")]
    //start delay variable
    [SerializeField] private float startDelay;
    [SerializeField] private bool isShrinking = false;
    [SerializeField] private float shrinkAmount;
    [SerializeField] private CharacterEntity[] characters;

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
        CharacterSelect.OnCharacterSelect += EventHandler;
        Debug.Log("Subscribed to event");

        //list of character positions found
        characters = FindObjectsOfType<CharacterEntity>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (isShrinking) {
            //lerp the scale of the object to shrink it
            StormScaleChange();
            //for each player in the scene
            foreach (CharacterEntity playerchar in characters) {
                //if the player is not in the collider
                if (!stormCollider.bounds.Contains(playerchar.transform.position)) {
                    //hurt em
                    Debug.Log("Player is in the storm");
                    DealDamage();
                }
            }
        }
    }

    protected override void DealDamage() {
        Runner.LagCompensation.OverlapSphere(transform.position, radius, player: Object.InputAuthority, hits, playerLayer, HitOptions.IgnoreInputAuthority);
        foreach (LagCompensatedHit hit in hits) {
            IHealthComponent healthComponent = hit.GameObject.GetComponentInParent<IHealthComponent>();

            if (healthComponent != null) {
                healthComponent.OnTakeDamage(damage);
                Debug.Log("player dealt " + damage);
            }
        }
    }

    private void EventHandler() {
        Invoke("ShrinkStorm", startDelay);
        Debug.Log("Timer Started");
    }

    //shrinking the storm over time
    private void ShrinkStorm() {
        isShrinking = true;
        Debug.Log(isShrinking);
    }
    
    private void StormScaleChange() {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero , shrinkAmount * Time.deltaTime);
    }
}