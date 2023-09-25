using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawn : MonoBehaviour
{
    public GameObject[] enemiesToSpawn;
    [SerializeField] private float spawnRadius;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnNow", 3, 4);
    }

    void SpawnNow()
    {
        float yRotation = Random.Range(0,360f);
        transform.rotation = Quaternion.Euler(Vector3.up * yRotation);
        Instantiate(enemiesToSpawn[Random.Range(0, enemiesToSpawn.Length)], transform.forward * Random.Range(0,spawnRadius), transform.rotation); 
    }

    void SetSpawnRadius (float radius)
    {
        spawnRadius = radius;
    }
}
