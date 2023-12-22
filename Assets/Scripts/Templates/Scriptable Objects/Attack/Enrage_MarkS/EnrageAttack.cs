using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnrageAttack : Attack {
    protected override void Start() {
        //find player transform on start
        playerTransform = GameObject.FindGameObjectWithTag("Participant").transform;
        this.transform.parent = playerTransform;
        //set to 0,0,0 to avoid weird rotation
        this.transform.localPosition = Vector3.zero;
        base.Start();
    }
}
