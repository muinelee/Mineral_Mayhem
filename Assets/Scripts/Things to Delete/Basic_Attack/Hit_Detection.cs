using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit_Detection : Attack
{
    public LayerMask enemyLayer;
    public float attackRange = 4f;
    public float radius = 3f;

    public Vector3 offset;

    protected override void OnEnable()
    {
        base.OnEnable();
        DetectHits();
    }

    public void DetectHits()
    {
        Vector3 position = transform.position + offset;
        Vector3 direction = transform.forward;

        RaycastHit[] hits = Physics.SphereCastAll(position, radius, transform.forward, attackRange, enemyLayer);
        foreach (RaycastHit hit in hits)
        {
            GameObject enemy = hit.collider.gameObject;
            if (enemy != null)
            {
                DealDamage(enemy);
            }
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
