using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Arena : NetworkBehaviour
{
    public static Arena Current { get; private set; } 
    public Transform[] spawnPoints;
    public GameObject healthPickup;
    public GameObject altPickup;    // at time of writing I forget name of other collectible in-game lol - there may also be more than one, can create list
    public GameObject core;

    public SO_ArenaDefinition definition;    //Will have information relative to the specific arena we are on (music/name/index/icon/image, etc)

    public SplineContainer spline;

    private void Awake()
    {
        Current = this;
        if (spline == null) spline = GetComponentInChildren<SplineContainer>();
        //  Give GameManager a reference to the Arena we're on;
        //GameManager.SetArena(this);
    }

    public override void Spawned()
    {
        base.Spawned();
        //RoundManager.Instance.MatchStartEvent += StartArenaCinematic; 
        // Custom Host functionality present here if need be:
        /*if (NetworkPlayer.Local.IsLeader)
        {

        }*/ 
    }

    private void OnDestroy()
    {
        GameManager.SetArena(null);
        //RoundManager.Instance.MatchStartEvent -= StartArenaCinematic;
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

        entity.Input.NetworkUser = player;
        player.GameState = NetworkPlayer.EnumGameState.CharacterSelection;
        player.Avatar = entity.Input;

        Debug.Log($"Spawning character for {player.playerName} as {entity.name}");
        entity.transform.name = $"Character ({player.playerName})";
    }
    private void StartArenaCinematic()
    {
        Debug.Log("Starting Arena Cinematic"); 
        if (NetworkPlayer.Local) NetworkCameraEffectsManager.instance.StartCinematic(NetworkPlayer.Local);
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z)) 
        {
            // Go to Player Camera (Top-Down View)
            StartArenaCinematic();  
        }
    }

    /// <summary>
    /// Returns the center point of the core's spawn path.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCenterCoreLocation()
    {
        return spline.EvaluatePosition(0.5f);
    }

    /// <summary>
    /// Returns a random spot along the core's spawn path.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetNewCoreSpawnLocation()
    {
        float randomLocation = Random.Range(0f, 1f);
        return spline.EvaluatePosition(randomLocation);
    }
}
