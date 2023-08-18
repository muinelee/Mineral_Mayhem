using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private GameObject currentTarget;
    public GameObject[] targets;
    private KnockbackController kbc;

    public float attackDistance = 1.0f;
    public float dodgeRollProbability = 0.2f;
    public float blockProbability = 0.2f;
    public float bigHitThreshold = 10f;
    public float hitStrength = 1.0f;
    public Vector3 attackerPosition;
    public float attackerHitStrength;

    private enum State { Idle, Run, Attack, HitReact, BigHitReact, Dodge, Block, Dead };
    private State currentState;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        kbc = GetComponent<KnockbackController>();
        currentState = State.Idle;

        //add participants to the targets array
        targets = GameObject.FindGameObjectsWithTag("Participant");
        ChooseTarget();


    }

    private void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.gameObject.transform.position);                                             // Calculate distance to the current target

        switch (currentState)
        {
            case State.Idle:
                if (distanceToTarget <= attackDistance)                                                                                    // If within attack range, switch to Attack state
                {
                    currentState = State.Attack;                                                                                           // Switch to Attack state if in range
                    anim.SetTrigger("Attack");
                }
                else
                {
                    currentState = State.Run;
                }
                break;

            case State.Run:
                agent.SetDestination(currentTarget.gameObject.transform.position);                                                                              // Set destination to current target's position
                break;

            case State.Attack:
                Attack();                                                                                                                  // Call Attack function to handle attack logic
                break;

            case State.HitReact:
                break;
            
            case State.BigHitReact:
                break;

            case State.Dodge:
                Dodge();                                                                                                                   // Call Dodge function to handle dodge roll logic
                break;

            case State.Block:
                Block();                                                                                                                   // Call Block function to handle block logic
                break;

            case State.Dead:
                Die();                                                                                                                     // Call Die function to handle death logic
                break;
        }
    }

    public IEnumerator AgentDisableCoroutine(float delay)
    { 
        yield return new WaitForSeconds(delay);
    }

    private void ChooseTarget()
    {
        int targetIndex = Random.Range(0, targets.Length);                                                                                 // Choose a random target from the available ones
        currentTarget = targets[targetIndex];                                                                                              // Set the current target
    }

    void Attack()
    { 
    
    }

    public void HitReaction(Vector3 attackerPosition, float damage)
    {
        
        if (damage >= bigHitThreshold)
        {
            currentState = State.BigHitReact;
            anim.SetTrigger("BigHitReact");
            kbc.ApplyKnockback(attackerPosition, attackerHitStrength, 0f);
        }
        else
        {
            currentState = State.HitReact;
            anim.SetTrigger("HitReact");
            kbc.ApplyKnockback(attackerPosition, attackerHitStrength, 0f);
        }
    }

    void Dodge()
    { 
    
    }

    void Block()
    { 
    
    }

    void Die()
    { 
    
    }




}
