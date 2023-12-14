using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkAttack : NetworkBehaviour
{
    private void OnEnable()
    {
    }
    
    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
