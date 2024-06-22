using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.Unicode;

public class DummySpawning : MonoBehaviour
{

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private NetworkObject trainingDummy;
    [SerializeField] private NetworkRunner Runner; 
    
    void Start()
    {
        Runner = GameObject.Find("NetworkRunner").GetComponent<NetworkRunner>();
        SpawnDummies(); 
    }

    private void SpawnDummies()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            Runner.Spawn(trainingDummy, spawnPoint.position, Quaternion.identity);
        }
    }
}
