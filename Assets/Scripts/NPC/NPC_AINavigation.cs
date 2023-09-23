using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class NPC_AINavigation : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private enum Behavior {Chase, Wander}
    private Behavior behavior;
    private Transform player;
    private bool canMove = true;
    
    [Header("HitStun Properties")]
    [SerializeField] private float hitStunDuration;
    private float hitStunTimer;

    [Header("Wander Properties")]
    [SerializeField] private float pauseDuration;
    [SerializeField] private float wanderRange;

    private NPC_Attack npcAttack;

    void Start()
    {
        if (!navMeshAgent) navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        if (!npcAttack) npcAttack = gameObject.GetComponent<NPC_Attack>();

        player = GameObject.FindGameObjectWithTag("Participant").GetComponent<Transform>();
        SetBehavior();

    }

    void Update()
    {
        Move();

        if (hitStunTimer < hitStunDuration) hitStunTimer += Time.deltaTime;
        if (hitStunTimer > hitStunDuration && !canMove) canMove = true;
    }

    void SetBehavior()
    {
        int randomBehavior = Random.Range(0,2);
        if (randomBehavior == 0) 
        {
            behavior = Behavior.Chase;
            npcAttack.target = player.GetComponent<Player_Core>();
        }

        else
        {
            behavior = Behavior.Wander;
            GetTarget();
        }
    }

    void Move()
    {
         if (navMeshAgent.enabled == true)
        {
            if (!canMove) navMeshAgent.destination = transform.position;
            else if (behavior == Behavior.Chase && npcAttack.canAttack) navMeshAgent.destination = player.position;
            else if (behavior == Behavior.Wander) WanderBehavior();
        }
    }

    void GetTarget()
    {
        if (navMeshAgent.enabled) navMeshAgent.destination = transform.position + new Vector3(Random.Range(-wanderRange, wanderRange), 0, Random.Range(-wanderRange, wanderRange));
    }

    void WanderBehavior()
    {
        if (Vector3.Distance(navMeshAgent.destination, transform.position) <= navMeshAgent.stoppingDistance) Invoke("GetTarget", pauseDuration);
    }

    public void DeathActivated()
    {
        navMeshAgent.enabled = false;
        this.enabled = false;
    }

    public void ApplyHitStun()
    {
        canMove = false;
        hitStunTimer = 0;
    }
}