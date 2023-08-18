using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KnockbackController : MonoBehaviour
{
    private NavMeshAgent agent;
    private CharacterController cc;
    public bool isPlayer;                                                                                                                     // We use this bool to determine whether or not we're using a NavMeshAgent or a CharacterController
    public float kbStrength = 1.0f;                                                                                                    // This is the strength of the knockback
    public float verticalKbForce = 1.0f;                                                                                               // This is the vertical force of the knockback
    public float baseKbDuration = 0.5f;                                                                                                // This is the duration of the knockback, which is adjustable
    public float kbDurationPerStrength = 0.1f;                                                                                                // This is the duration of the knockback per strength, which is adjustable

    private void Start()
    {
        if (isPlayer)
        {
            cc = GetComponent<CharacterController>();
            agent = null;
        }
        else
        {
            agent = GetComponent<NavMeshAgent>();
            cc = null;
        }
    }

    public void ApplyKnockback(Vector3 attackerPosition, float hitStrength, float verticalKnockback = 0f)
    {
        verticalKnockback = (verticalKnockback == 0f) ? verticalKbForce : verticalKnockback;                                           // If we don't pass in a vertical knockback force, we use the default value
        Vector3 direction = transform.position - attackerPosition;                                                                            // We get the direction of the knockback
        direction.y = 0f;                                                                                                                     // We set the y value to 0 so that we don't knock the player into the air
        direction.Normalize();                                                                                                                // We normalize the direction so that we can multiply it by the knockback strength
        direction *= hitStrength * kbStrength;                                                                                         // We multiply the direction by the knockback strength
        direction.y = verticalKnockback;                                                                                                      // We set the y value to the vertical knockback force

        float duration = baseKbDuration + (hitStrength * kbDurationPerStrength);                                                              // We calculate the duration of the knockback
        
        if (isPlayer)
        {
            cc.enabled = false;                                                                                                               // We disable the CharacterController so that we can move the player
            cc.Move(direction * Time.deltaTime);                                                                                              // We move the player using the CharacterController
            StartCoroutine(StunCoroutine(duration));
        }
        else
        {
            agent.enabled = false;                                                                                                            // We disable the NavMeshAgent so that we can move the enemy
            transform.position += direction * Time.deltaTime;                                                                                 // We move the enemy using the transform
            StartCoroutine(StunCoroutine(duration));
        }
    }

    private IEnumerator StunCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);                                                                                            // We wait for the duration of the knockback
        if (isPlayer)
        {
            cc.enabled = true;                                                                                                                // We re-enable the CharacterController
        }
        else
        { 
            agent.enabled = true;                                                                                                             // We re-enable the NavMeshAgent
        }
    }
}
