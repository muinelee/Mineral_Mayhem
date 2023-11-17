using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private HashSet<GameObject> enemies = new HashSet<GameObject>();
    public int amountKilled = 0;
    //public GameObject[] enemies;
    public bool subscribed = false;

    public static GameManager instance;
    public UIManager uiManager;

    private void Start()
    {
        FindAndSubcribeToEnemies();
    }
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        FindAndSubcribeToEnemies();
/*        //find all objects with NPC_Core script
        enemies = GameObject.FindGameObjectsWithTag("NPC");
        //subscribe to death function
        foreach (GameObject enemy in enemies)
            {
                //find the script attached to the object
                NPC_Core npcCore = enemy.GetComponent<NPC_Core>();
                if (npcCore != null)
                {
                    //subscribe to the death function
                    npcCore.OnDeath += AddToAmountKilled;
                }
            }
        //testing
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("The number of enemies in length is" + enemies.Length);
        }*/
    }

    private void FindAndSubcribeToEnemies()
    {
        GameObject[] foundNPCs = GameObject.FindGameObjectsWithTag("NPC");

        //iterate through the array
        foreach (GameObject enemy in foundNPCs)
        {
            //check if enemy is already in the set
            if (enemies.Contains(enemy))
            {
                continue;
            }

            //find script attached to enemy
            NPC_Core npcCore = enemy.GetComponent<NPC_Core>();
            if (npcCore != null)
            {
                //subscribe to the death function
                npcCore.OnDeath += AddToAmountKilled;
            }

            //add enemy to the set
            enemies.Add(enemy);
        }
    }
    void AddToAmountKilled()
    {
        amountKilled++;
        Debug.Log("Amount Killed: " + amountKilled);
    }

/*    public void AddEnemy(GameObject enemy)
    {
        GameObject[] newEnemies = new GameObject[enemies.Length + 1];

        //copy the old array into the new array
        for (int i = 0; i < enemies.Length; i++)
        {
            newEnemies[i] = enemies[i];
        }

        //add the enemy to the end of the array
        newEnemies[newEnemies.Length] = enemy;

        //update the array
        enemies = newEnemies;
    }*/

}
