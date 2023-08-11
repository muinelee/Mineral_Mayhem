using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject npcPrefab;
    public CinemachineVirtualCamera virtualCameraPrefab;
    public Transform[] spawnPoints;
    public int desiredTotalParticipants = 4;

    private GameObject[] participants;
    private CinemachineVirtualCamera[] virtualCameras;
    public int playerCount = 1;

    void Start()
    {
        participants = new GameObject[desiredTotalParticipants];                                                                                // We populate the participants array with the desired total participants
        virtualCameras = new CinemachineVirtualCamera[desiredTotalParticipants];                                                                // We populate the virtualCameras array with the total participants to be used for the virtual cameras assignment

        StartGame();                                                                                                                            // We call the StartGame() function, to be used for instantiation of the players and NPCs
    }

    void StartGame()
    {
        for (int i = 0; i < playerCount; i++)
        {
            participants[i] = InstantiatePlayer(i);                                                                                             // Instantiate the amount of players equal to those that have joined the game session. *Multiplayer in mind*    
        }

        for (int i = playerCount; i < desiredTotalParticipants; i++)
        {
            participants[i] = InstantiateNPC(i);                                                                                                // Fill the remaining participants array with NPCs to ensure consistent gameplay. *Multiplayer in mind*
        }
    }



    GameObject InstantiatePlayer(int playerIndex)
    {
        Transform spawnTransform = spawnPoints[playerIndex];                                                                                    // We assign the spawnTransform to the spawnPoint at the index of the playerIndex
        GameObject player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);                                        // We instantiate the player at the spawnTransform position and rotation
        PlayerController pc = player.GetComponent<PlayerController>();                                                                          // We get the PlayerController component from the player
        pc.PlayerNumber = (playerIndex + 1);                                                                                                    // We assign the player number to the playerIndex + 1, to ensure that the player number is never 0

        CinemachineVirtualCamera vCam = Instantiate(virtualCameraPrefab);                                                                       // We instantiate the virtual camera
        vCam.Follow = player.transform;                                                                                                         // We assign the virtual camera to follow the player
        virtualCameras[playerIndex] = vCam;                                                                                                     // We assign the virtual camera to the virtualCameras array at the index of the playerIndex                                                                                                              // We assign the virtual camera to the player's virtualCamera variable
        vCam.Priority = 15;                                                                                                                     // We set the priority of the virtual camera to 15, to ensure that it is the main camera for the client *Multiplayer in mind*
        return player;                                                                                                                          // We return the player with all the information assembled above
    }

    GameObject InstantiateNPC(int npcIndex)
    {
        Transform spawnTransform = spawnPoints[npcIndex];                                                                                       // We assign the spawnTransform to the spawnPoint at the index of the npcIndex
        return Instantiate(npcPrefab, spawnTransform.position, spawnTransform.rotation);                                                        // We instantiate the NPC at the spawnTransform position and rotation

    }

    public void OnPlayerJoined()                                                                                                                // This function is called when a player joins the game session
    {
        playerCount++;
    }

}
