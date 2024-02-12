using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : NetworkSceneManagerBase
{
    public const int ARENA_PREGAME = 6;
    public const int ARENA_TEST = 7;
    public static LevelManager Instance => Singleton<LevelManager>.Instance;

    public static void LoadMenu()
    {
        Instance.Runner.SetActiveScene(ARENA_PREGAME);
    }

    public static void LoadScene(int sceneIndex)
    {
        Instance.Runner.SetActiveScene(sceneIndex);
    }

    protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
    {
        //  FOR FUTURE STUFFS!!!
        //  Preload new scene                           (ie, PreLoadScene(newScene);)
        //  Make an empty list of networked objects     (List<NetworkObject> sceneObjects = new List<NetworkObject>();)
        //  If new scene is a game scene (ie, not lobby/main menu/whatever else -> Load scene async & add sceneObjects to list
        //  run the ''finished'' delegate over the list of objects, if need be
        List<NetworkObject> sceneObjects = new List<NetworkObject>();

        if (newScene >= ARENA_PREGAME)
        {
            yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);
            Scene loadedScene = SceneManager.GetSceneByBuildIndex(newScene);
            Debug.Log($"Loaded scene {newScene}: {loadedScene}");
            sceneObjects = FindNetworkObjects(loadedScene, disable: false);
        }

        finished(sceneObjects);

        //  Delay 1 frame for safety
        yield return null;

        Debug.Log("Checking spawn logic");
        //  Spawn Character Logic Here  -   Edit as need be
        if (GameManager.CurrentArena != null && newScene >= ARENA_PREGAME)
        {
            Debug.Log("Checking host credentials");
            if (Runner.GameMode == GameMode.Host)
            {
                Debug.Log("Trying to spawn per player");
                foreach (var player in NetworkPlayer.Players)
                {
                    Debug.Log("Do the spawn thing");
                    player.GameState = NetworkPlayer.EnumGameState.CharacterSelection;
                    GameManager.CurrentArena.SpawnCharacter(Runner, player);
                }
            }
        }

        //  Postload new scene                          (ie, PostLoadScene();)
    }

    private void PreLoadScene(int scene)
    {
        // Do Preload stuff - maybe add a fader, change camera locations, have some chicken, maybe some se#... wait wut?
    }

    private void PostLoadScene()
    {
        // Do Postload stuff - maybe unfade, or... I dunno. Variety of stuffs 
    }
}
