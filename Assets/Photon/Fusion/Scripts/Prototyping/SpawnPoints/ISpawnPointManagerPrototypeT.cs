
using UnityEngine;
using Fusion;

/// <summary>
/// Interface for <see cref="SpawnPointManagerPrototype{T}"/> behaviours.
/// </summary>
public interface ISpawnPointManagerPrototype<T> where T : Component, ISpawnPointPrototype {
  void CollectSpawnPoints(Fusion.NetworkRunner runner);
  Transform GetNextSpawnPoint(Fusion.NetworkRunner runner, PlayerRef player, bool skipIfBlocked = true);
}

