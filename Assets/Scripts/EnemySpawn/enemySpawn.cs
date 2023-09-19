using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawn : MonoBehaviour
{

    public GameObject[] enemiesToSpawn; 
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnNow", 3, 4); 
    }

    Vector3 getRandomPos()
    {
        float _x = Random.Range(-18, 18);
        float _y = 0.5f;
        float _z = Random.Range(-20, 20);

        Vector3 newPos = new Vector3(_x, _y, _z);
        return newPos; 
    }

    void SpawnNow()
    {
        Instantiate(enemiesToSpawn[Random.Range(0, 2)], getRandomPos(), Quaternion.identity); 
    }
}
