using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    public LayerMask enemyLayer;
    public float attackRange = 4f;
    public float radius = 3f;
    public Vector3 offset;

    public void DetectHits()
    {
        Debug.Log("Detecting hits");
        Vector3 position = transform.position + offset;
        Vector3 direction = transform.forward;

        RaycastHit[] hits = Physics.SphereCastAll(position, radius, direction, attackRange, enemyLayer);
        foreach (RaycastHit hit in hits)
        {
            GameObject enemy = hit.collider.gameObject;
            if (enemy != null)
            {
/*                Debug.Log("Hit " + hit.collider.name);
                if (enemy.CompareTag("NPC"))
                {
                    Debug.Log("Hit " + hit.collider.name);
                    enemy.GetComponent<NPCDefense>().TakeDamage(10f, 10f, transform.position, STATUS_EFFECT.NONE, 0, 0);
                }
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
