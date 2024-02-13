using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterChangeTest : MonoBehaviour
{
    [SerializeField] private CharacterSpawner spawner;
    [SerializeField] private NetworkPlayer player;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Changing character to X Bot");
            spawner.playerPrefab = player;
        }
    }
}
