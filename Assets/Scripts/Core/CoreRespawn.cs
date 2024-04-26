using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreRespawn : MonoBehaviour
{
    [SerializeField] private RoundManager roundManager;
    [SerializeField] private Transform respawnPoint;
    private bool hasRespawned;

    private void Start()
    {
        if (roundManager != null)
        {
            roundManager.MatchStartEvent += OnMatchStart;
            roundManager.ResetRound += RespawnPlayerCore;
        }
    }

    private void OnDestroy()
    {
        if (roundManager != null)
        {
            roundManager.MatchStartEvent -= OnMatchStart;
            roundManager.ResetRound -= RespawnPlayerCore;
        }
    }

    private void OnMatchStart()
    {
        if (!hasRespawned)
        {
            RespawnPlayerCore();
            hasRespawned = true;
        }
    }

    private void RespawnPlayerCore()
    {
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            transform.rotation = respawnPoint.rotation;
        }
        else
        {
            Debug.LogError("Respawn point not set for PlayerCoreRespawn script!");
        }
    }
}