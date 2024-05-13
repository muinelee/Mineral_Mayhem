using Fusion.Editor;
using UnityEditor;
using UnityEngine;
using Photon;

[InitializeOnLoad]
public class RebuildFusionTable : Editor
{
    static RebuildFusionTable()
    {
        AssemblyReloadEvents.beforeAssemblyReload += OnAssemblyReload;
    }

    private static void OnAssemblyReload()
    {
        Fusion.Editor.NetworkProjectConfigUtilities.RebuildPrefabTable();
        Debug.Log("Table rebuilt");
        AssetDatabase.Refresh();
    }
}
