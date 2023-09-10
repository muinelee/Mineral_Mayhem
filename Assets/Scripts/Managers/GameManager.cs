using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int amountKilled = 0;
    public GameObject[] enemies;
    public bool subscribed = false;

    public static GameManager instance;

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
        if (!subscribed)
        {
            //find all objects with NPC_Core script
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
            subscribed = true;
        }
        //testing
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("The number of enemies in length is" + enemies.Length);
        }
    }

    void AddToAmountKilled()
    {
        amountKilled++;
        Debug.Log("Amount Killed: " + amountKilled); //fuck
    }
}
