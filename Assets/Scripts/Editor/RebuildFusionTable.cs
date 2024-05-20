using Fusion.Editor;
using UnityEditor;
using UnityEngine;
using Photon;

[InitializeOnLoad]
public class RebuildFusionTable : Editor
{
    static RebuildFusionTable()
    {
        EditorApplication.update += OnAssemblyReload;
    }

    private static void OnAssemblyReload()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
        {
            Fusion.Editor.NetworkProjectConfigUtilities.RebuildPrefabTable();
            Debug.Log("Table rebuilt");
            AssetDatabase.Refresh();
            EditorApplication.update -= OnAssemblyReload;
        }
    }
}