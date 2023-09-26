using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawn : MonoBehaviour
{
    public GameObject[] enemiesToSpawn;
    [SerializeField] private float[] spawnRadius;

    //FIX BEFORE FINAL BUILD
    private float timer = 0;
    private int spawnRadiusCounter;
    //

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnNow", 3, 4);
    }

    private void Update() 
    {
        timer += Time.deltaTime;
    }

    void SpawnNow()
    {
        float yRotation = Random.Range(0,360f);
        transform.rotation = Quaternion.Euler(Vector3.up * yRotation);

        //DELETE IN FINAL BUILD
        spawnRadiusCounter = Mathf.RoundToInt(timer / 30f);
        //

        Instantiate(enemiesToSpawn[Random.Range(0, enemiesToSpawn.Length)], transform.forward * Random.Range(0,spawnRadius[spawnRadiusCounter]), transform.rotation); 
    }

/*
    void SetSpawnRadius (float radius)
    {
        spawnRadius = radius;
    }
    */
}
