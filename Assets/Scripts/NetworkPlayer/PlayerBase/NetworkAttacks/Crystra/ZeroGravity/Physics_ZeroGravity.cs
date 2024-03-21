using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics_ZeroGravity : NetworkAttack_Base
{
    [Header("Spell Properties")]
    [SerializeField] private float lifetime;
    [SerializeField] private Vector3 offset;

    private float lifeTimer = 0;

    private void Start()
    {
        transform.position += transform.up * offset.y + transform.forward * offset.z;
    }

    private void FixedUpdate()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer > lifetime) Destroy(gameObject);
    }
}
