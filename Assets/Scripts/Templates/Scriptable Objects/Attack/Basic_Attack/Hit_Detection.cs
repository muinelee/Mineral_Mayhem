using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit_Detection : Attack
{
    public LayerMask enemyLayer;
    public float attackRange = 4f;
    public float radius = 3f;

    // ---- Delete in final build
    public float timer = 0;
    // -----

    public Vector3 offset;

    private void Start() 
    {
        DetectHits();
    }

    private void Update() {
        timer += Time.deltaTime;
        if (timer > 0.5f) Destroy(gameObject);
    }

    public void DetectHits()
    {
        Vector3 position = transform.position + offset;
        Vector3 direction = transform.forward;

        RaycastHit[] hits = Physics.SphereCastAll(position, radius, direction, attackRange, enemyLayer);
        foreach (RaycastHit hit in hits)
        {
            GameObject enemy = hit.collider.gameObject;
            if (enemy != null)
            {
                if (enemy.CompareTag("NPC"))
                {
                    enemy.GetComponent<NPC_Core>().TakeDamage(attackDamage, attackKnockback, playerTransform.position, STATUS_EFFECT.NONE, 0, 0);
                }
                /*
                else if (enemy.CompareTag("Player"))

                {
                    Debug.Log("Hit " + hit.collider.name);
                    enemy.GetComponent<PlayerDefense>().TakeDamage(10f, 10f, transform.position, STATUS_EFFECT.NONE, 0, 0);
                }
                */
            }
        }
    }
}
