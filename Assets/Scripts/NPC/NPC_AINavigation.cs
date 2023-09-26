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
    [SerializeField] private LayerMask stageLayer;
    [SerializeField] private Transform groundCheck;

    private NPC_Attack npcAttack;
    private Rigidbody rb;
    [SerializeField] private float gravityScale;

    void Start()
    {
        if (!navMeshAgent) navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        if (!npcAttack) npcAttack = gameObject.GetComponent<NPC_Attack>();
        if (!rb) rb = gameObject.GetComponent<Rigidbody>();

        player = GameObject.FindGameObjectWithTag("Participant").GetComponent<Transform>();
        SetBehavior();
    }

    void Update()
    {
        Move();

        if (hitStunTimer < hitStunDuration) hitStunTimer += Time.deltaTime;
        if (hitStunTimer > hitStunDuration && !canMove) canMove = true;

        if (!Physics.Raycast(groundCheck.position, Vector3.down, 5, stageLayer)) 
        {
            navMeshAgent.enabled = false;
            rb.AddForce(Vector3.down * 9.81f * gravityScale, ForceMode.Force);
        }

        if (!Physics.Raycast(groundCheck.position, groundCheck.forward, 10, stageLayer) && behavior == Behavior.Wander) GetTarget();
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
            else if (behavior == Behavior.Chase && npcAttack.canAttack && player) navMeshAgent.destination = player.position;
            else if (behavior == Behavior.Wander) WanderBehavior();
        }
    }

    void GetTarget()
    {
        if (navMeshAgent.enabled) 
        {
            Vector3 newTargetDestination = transform.position + new Vector3(Random.Range(-wanderRange, wanderRange), 0, Random.Range(-wanderRange, wanderRange));
            navMeshAgent.destination = newTargetDestination;
        }
    }

    void WanderBehavior()
    {
        if (Vector3.Distance(navMeshAgent.destination, transform.position) <= navMeshAgent.stoppingDistance) Invoke("GetTarget", pauseDuration);
    }

    private void OnDisable() 
    {
        if (navMeshAgent.enabled == true) navMeshAgent.enabled = false;
    }

    private void OnEnable() 
    {
        if (navMeshAgent && navMeshAgent.enabled == false) navMeshAgent.enabled = true;
    }

    public void ApplyHitStun()
    {
        canMove = false;
        hitStunTimer = 0;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawRay(groundCheck.position, groundCheck.forward * 10);
    }
}