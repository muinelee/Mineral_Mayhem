using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : NetworkSceneManagerBase
{
    public string tempSceneReference = "RichardCPhoton";
    public static LevelManager Instance => Singleton<LevelManager>.Instance;

    protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
    {
        //  FOR FUTURE STUFFS!!!
        //  Preload new scene                           (ie, PreLoadScene(newScene);)
        //  Make an empty list of networked objects     (List<NetworkObject> sceneObjects = new List<NetworkObject>();)
        //  If new scene is a game scene (ie, not lobby/main menu/whatever else -> Load scene async & add sceneObjects to list
        //  run the ''finished'' delegate over the list of objects, if need be

        //  Delay 1 frame for safety
        yield return null;

        //  Spawn Character Logic Here  -   Edit as need be
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("RichardCPhoton"))
        {
            if (Runner.GameMode == GameMode.Host)
            {
                foreach (var player in NetworkPlayer.Players)
                {
                    player.GameState = NetworkPlayer.EnumGameState.CharacterSelection;
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
