using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Diagnostics;

public class ShrinkingStorm : NetworkBehaviour {

    [Header("Shrinking Storm Variables")]
    //timer for start delay
    private TickTimer shrinkTimer = TickTimer.None;
    //var to hold remaining time
    private float remainingTime;

    [Header("Other Variables")]
    [SerializeField] private float lifeTime;
    //start delay variable
    [SerializeField] private float startDelay;


    // Start is called before the first frame update
    void Start() {
        //start the timer with the start delay
        shrinkTimer = TickTimer.CreateFromSeconds(Runner, startDelay);
    }

    // Update is called once per frame
    void FixedUpdate() {
        //if start delay timer has expired, start the shrinking storm
        if (shrinkTimer.Expired(Runner)) {
            shrinkTimer = TickTimer.None;
            //set the timer to the life time of the storm
            shrinkTimer = TickTimer.CreateFromSeconds(Runner, lifeTime);
            UnityEngine.Debug.Log("shrinkTimer.RemainingTime(Runner);" + shrinkTimer.RemainingTime(Runner));
        }
    }

    //shrinking the storm over time
    private void ShrinkStorm() {

    }
}
