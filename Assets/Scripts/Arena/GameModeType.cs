using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Mode", menuName = "Scriptable Object/Game Mode Type")]
public class GameModeType : ScriptableObject
{
    //  Definition for holding the game mode information. Can customize alongside Arena Definition for same game modes on different maps.
    public string modeName;
    public bool hasPickups;
    public bool hasOrb;
    public bool hasTeams;
    public bool hasRespawns;
}
