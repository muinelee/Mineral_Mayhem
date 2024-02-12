using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    //  Feel free to use ths for references to UI prefabs that will be important later
    public SO_Character[] charDefinitions;
    public ArenaDefinition[] arenas;
    public GameModeType[] gameModes;

    public static ResourceManager Instance => Singleton<ResourceManager>.Instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);   
    }

}
