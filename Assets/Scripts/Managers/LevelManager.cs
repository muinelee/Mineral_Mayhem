using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Fusion.Behaviour, INetworkSceneManager
{
    private static WeakReference<LevelManager> IsCurrentlyLoading = new WeakReference<LevelManager>(null);

    [InlineHelp]
    [ToggleLeft]
    [MultiPropertyDrawersFix]
    public bool ShowHierarchyWindowOverlay = true;

    private Task SwitchSceneTask;
    private bool CurrentSceneOutdated;
    protected SceneRef CurrentScene;

    public NetworkRunner Runner { get; private set; }

    public const int ARENA_MAIN = 3;
    public const int ARENA_TEST = 4;
    public static LevelManager Instance => Singleton<LevelManager>.Instance;

    protected virtual void OnEnable()
    {
#if UNITY_EDITOR
        if (ShowHierarchyWindowOverlay)
        {
            UnityEditor.EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowOverlay;
        }
#endif
    }

    protected virtual void OnDisable()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.hierarchyWindowItemOnGUI -= HierarchyWindowOverlay;
#endif
    }

    protected virtual void Initialize(NetworkRunner runner)
    {
        Assert.Check(!Runner);
        Runner = runner;
    }

    protected virtual void Shutdown(NetworkRunner runner)
    {
        Assert.Check(Runner == runner);
        try
        {
            if (SwitchSceneTask != null && SwitchSceneTask.Status == TaskStatus.Running)
            {
                LogWarn($"There is an ongoing scene load ({CurrentScene}), stopping and disposing of the coroutine.");
                (SwitchSceneTask as IDisposable)?.Dispose();
            }
        }
        finally
        {
            Runner = null;
            SwitchSceneTask = null;
            CurrentScene = SceneRef.None;
            CurrentSceneOutdated = false;
        }
    }

    protected virtual async Task<IEnumerable<NetworkObject>> SwitchScene(SceneRef prevScene, SceneRef newScene, bool additive)
    {
        prevScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1).buildIndex;
        Debug.Log($"Switching Scene from {prevScene} to {newScene}");

        List<NetworkObject> sceneObjects = new List<NetworkObject>();
        if (newScene >= 0)
        {

            if (additive)
            {
                var loadedScene = await LoadSceneAsset(newScene, LoadSceneMode.Additive);
                Debug.Log($"Loaded scene {newScene}: {loadedScene}");
                sceneObjects = FindNetworkObjects(loadedScene, disable: false);
            }
            else
            {
                var loadedScene = await LoadSceneAsset(newScene, LoadSceneMode.Single);
                Debug.Log($"Loaded scene {newScene}: {loadedScene}");
                sceneObjects = FindNetworkObjects(loadedScene, disable: false);
            }
        }

        Debug.Log($"Switched Scene from {prevScene} to {newScene} - loaded {sceneObjects.Count} objects");
        return sceneObjects;
    }

    private async Task<Scene> LoadSceneAsset(int sceneIndex, LoadSceneMode mode)
    {
        var scene = new Scene();
        var op = await SceneManager.LoadSceneAsync(sceneIndex, mode);
        op.completed += (operation) => scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        return scene;
    }

    public List<NetworkObject> FindNetworkObjects(Scene scene, bool disable = true, bool addVisibilityNodes = false)
    {

        var networkObjects = new List<NetworkObject>();
        var gameObjects = scene.GetRootGameObjects();
        var result = new List<NetworkObject>();

        // get all root gameobjects and move them to this runners scene
        foreach (var go in gameObjects)
        {
            networkObjects.Clear();
            go.GetComponentsInChildren(true, networkObjects);

            foreach (var sceneObject in networkObjects)
            {
                if (sceneObject.Flags.IsSceneObject())
                {
                    if (sceneObject.gameObject.activeInHierarchy || sceneObject.Flags.IsActivatedByUser())
                    {
                        Assert.Check(sceneObject.NetworkGuid.IsValid);
                        result.Add(sceneObject);
                    }
                }
            }

            if (addVisibilityNodes)
            {
                // register all render related components on this gameobject with the runner, for use with IsVisible
                RunnerVisibilityNode.AddVisibilityNodes(go, Runner);
            }
        }

        if (disable)
        {
            // disable objects; each will be activated if there's a matching state object
            foreach (var sceneObject in result)
            {
                sceneObject.gameObject.SetActive(false);
            }
        }

        return result;
    }

    #region Interface Functions
    void INetworkSceneManager.Initialize(NetworkRunner runner)
    {
        Initialize(runner);
    }

    void INetworkSceneManager.Shutdown(NetworkRunner runner)
    {
        Shutdown(runner);
    }

    bool INetworkSceneManager.IsReady(NetworkRunner runner)
    {
        Assert.Check(Runner == runner);
        if (SwitchSceneTask != null && SwitchSceneTask.Status == TaskStatus.Running || !Runner)
        {
            return false;
        }
        if (CurrentSceneOutdated)
        {
            return false;
        }

        return true;
    }
    #endregion

    #region Diagnostics
    [System.Diagnostics.Conditional("FUSION_NETWORK_SCENE_MANAGER_TRACE")]
    protected void LogTrace(string msg)
    {
        Log.Debug($"[NetworkSceneManager] {(this != null ? this.name : "<destroyed>")}: {msg}");
    }

    protected void LogError(string msg)
    {
        Log.Error($"[NetworkSceneManager] {(this != null ? this.name : "<destroyed>")}: {msg}");
    }

    protected void LogWarn(string msg)
    {
        Log.Warn($"[NetworkSceneManager] {(this != null ? this.name : "<destroyed>")}: {msg}");
    }
    #endregion

    private async Task SwitchSceneWrapper(SceneRef prevScene, SceneRef newScene, bool additive)
    {
        IEnumerable<NetworkObject> sceneObjects;
        Exception error = null;

        try
        {
            Assert.Check(!IsCurrentlyLoading.TryGetTarget(out _));
            IsCurrentlyLoading.SetTarget(this);
            Runner.InvokeSceneLoadStart();
            sceneObjects = await SwitchScene(prevScene, newScene, additive);
        }
        catch (Exception ex)
        {
            sceneObjects = null;
            error = ex;
        }
        finally
        {
            Assert.Check(IsCurrentlyLoading.TryGetTarget(out var target) && target == this);
            IsCurrentlyLoading.SetTarget(null);
            SwitchSceneTask = null;
            LogTrace($"Coroutine finished for {newScene}");
        }

        if (error != null)
        {
            LogError($"Failed to switch scenes: {error}");
        }
        else
        {
            Runner.RegisterSceneObjects(sceneObjects);
            Runner.InvokeSceneLoadDone();
        }
    }

    public static void LoadScene(int sceneIndex, bool additive)
    {
        if (!Instance.Runner) return;
        if (Instance.SwitchSceneTask != null && Instance.SwitchSceneTask.Status != TaskStatus.Running)
            //busy
            return;

        if (IsCurrentlyLoading.TryGetTarget(out var target))
        {
            Assert.Check(target != Instance);
            if (!target)
            {
                // orphaned loader?
                IsCurrentlyLoading.SetTarget(null);
            }
            else
            {
                Instance.LogTrace($"Waiting for {target} to finish loading");
                return;
            }
        }
        var prevScene = Instance.CurrentScene;
        Instance.LogTrace($"Scene transition {prevScene}->{Instance.CurrentScene}");
        Instance.SwitchSceneTask = Instance.SwitchSceneWrapper(prevScene, Instance.CurrentScene, additive);
    }

    // Outdated, but used as a reference for now
   /* protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
    {
        //  FOR FUTURE STUFFS!!!
        //  Preload new scene                           (ie, PreLoadScene(newScene);)
        //  Make an empty list of networked objects     (List<NetworkObject> sceneObjects = new List<NetworkObject>();)
        //  If new scene is a game scene (ie, not lobby/main menu/whatever else -> Load scene async & add sceneObjects to list
        //  run the ''finished'' delegate over the list of objects, if need be
        List<NetworkObject> sceneObjects = new List<NetworkObject>();

        if (newScene >= ARENA_MAIN)
        {
            yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);
            Scene loadedScene = SceneManager.GetSceneByBuildIndex(newScene);
            Debug.Log($"Loaded scene {newScene}: {loadedScene}");
            sceneObjects = FindNetworkObjects(loadedScene, disable: false);
        }

        finished(sceneObjects);

        //  Delay 1 frame for safety
        yield return null;

        //  Spawn Character Logic Here  -   Edit as need be
        if (GameManager.CurrentArena != null && newScene >= ARENA_MAIN)
        {
            if (Runner.GameMode == GameMode.Host)
            {
                foreach (var player in NetworkPlayer.Players)
                {
                    player.GameState = NetworkPlayer.EnumGameState.CharacterSelection;
                    GameManager.CurrentArena.SpawnCharacter(Runner, player);
                }
            }
        }

        //  Postload new scene                          (ie, PostLoadScene();)
    }*/

#if UNITY_EDITOR
    private static Lazy<GUIStyle> s_hierarchyOverlayLabelStyle = new Lazy<GUIStyle>(() => {
        var result = new GUIStyle(UnityEditor.EditorStyles.miniButton);
        result.alignment = TextAnchor.MiddleCenter;
        result.fontSize = 9;
        result.padding = new RectOffset(4, 4, 0, 0);
        result.fixedHeight = 13f;
        return result;
    });

    private void HierarchyWindowOverlay(int instanceId, Rect position)
    {
        if (!Runner)
        {
            return;
        }

        if (!Runner.MultiplePeerUnityScene.IsValid())
        {
            return;
        }

        if (Runner.MultiplePeerUnityScene.GetHashCode() == instanceId)
        {

            var rect = new Rect(position)
            {
                xMin = position.xMax - 56,
                xMax = position.xMax - 2,
                yMin = position.yMin + 1,
            };

            if (GUI.Button(rect, $"{Runner.Mode} {(Runner.LocalPlayer.IsValid ? "P" + Runner.LocalPlayer.PlayerId.ToString() : "")}", s_hierarchyOverlayLabelStyle.Value))
            {
                UnityEditor.EditorGUIUtility.PingObject(Runner);
                UnityEditor.Selection.activeGameObject = Runner.gameObject;
            }
        }
    }
#endif
}
