using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : NetworkBehaviour
{
    public static Arena Current { get; private set; }

    public Transform[] spawnPoints;
    public GameObject healthPickup;
    public GameObject altPickup;    // at time of writing I forget name of other collectible in-game lol - there may also be more than one, can create list
    public GameObject core;

    public ArenaDefinition definition;    //Will have information relative to the specific arena we are on (music/name/index/icon/image, etc)


    private void Awake()
    {
        Current = this;

        //  Give GameManager a reference to the Arena we're on;
        GameManager.SetArena(this);
    }

    public override void Spawned()
    {
        base.Spawned();
        // Custom Host functionality present here if need be:
        /*if (NetworkPlayer.Local.IsLeader)
        {

        }*/
    }

    private void OnDestroy()
    {
        GameManager.SetArena(null);
    }

    public void SpawnCharacter(NetworkRunner runner, NetworkPlayer player)
    {
        var index = NetworkPlayer.Players.IndexOf(player);
        var point = spawnPoints[index];

        var prefabID = player.CharacterID;
        var prefab = ResourceManager.Instance.charDefinitions[prefabID].prefab;

        // Spawns player character
        var entity = runner.Spawn(
            prefab,
            point.position,
            point.rotation,
            player.Object.InputAuthority);

        entity.Controller.NetworkUser = player;
        player.GameState = NetworkPlayer.EnumGameState.CharacterSelection;
        player.Avatar = entity.Controller;

        Debug.Log($"Spawning character for {player.playerName} as {entity.name}");
        entity.transform.name = $"Character ({player.playerName})";
    }
}
