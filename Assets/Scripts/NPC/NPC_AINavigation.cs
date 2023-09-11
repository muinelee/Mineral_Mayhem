using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class NPC_AINavigation : MonoBehaviour
{
    private NavMeshAgent nma;
    private enum Behavior {Chase, Wander}
    private Behavior behavior;

    private Transform player;

    [SerializeField] private float pauseDuration;

    [SerializeField] private float distanceOffset;

    void Start()
    {
        if (transform.GetComponent<NavMeshAgent>()) nma = transform.GetComponent<NavMeshAgent>();

        SetBehavior();

        player = GameObject.FindGameObjectWithTag("Participant").GetComponent<Transform>();
    }

    void Update()
    {
        Move();
    }

    void SetBehavior()
    {
        int randomBehavior = Random.Range(0,2);
        if (randomBehavior == 0) behavior = Behavior.Chase;
        
        else
        {
            behavior = Behavior.Wander;
            GetTarget();
        }
    }

    void Move()
    {
        if (behavior == Behavior.Chase) 
        {
            nma.destination = player.position;    // Only applies to chasing enemies
        }
            
        else if (behavior == Behavior.Wander) WanderBehavior();

            // ----- Chase Behavior -----

            // ----- After hitting player change target Need Attack Script



            // ----- Wander Behavior -----

            // Get Target when starting

            

            //  Change target when reaching target
    }

    void GetTarget()
    {
        nma.destination = transform.position + new Vector3(Random.Range(-distanceOffset, distanceOffset), 0, Random.Range(-distanceOffset, distanceOffset));
    }

    void WanderBehavior()
    {
        if (Vector3.Distance(nma.destination, transform.position) <= nma.stoppingDistance) 
        {
            Invoke("GetTarget", pauseDuration);
        }
    }
}