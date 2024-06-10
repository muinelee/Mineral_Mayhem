using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class SceneLoader : Editor
{
    static SceneLoader()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
    }

    private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        switch(scene.buildIndex)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                EditorSceneManager.OpenScene("Assets/_Scenes/Level Design/AbigailMckellarLD.unity", OpenSceneMode.Additive);
                break;
        }
    }
}
