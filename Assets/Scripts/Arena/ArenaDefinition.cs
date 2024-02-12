using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Arena", menuName = "Scriptable Object/Arena Definition")]
public class ArenaDefinition : ScriptableObject
{
    //  Definition for holding the Arena information. Can complement with GameModeType for different and varying game types on same maps
    public string arenaName;
    public int buildIndex;
    public string bgMusic;
    public Sprite arenaIcon;
}
