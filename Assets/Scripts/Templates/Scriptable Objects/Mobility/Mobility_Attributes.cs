using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mobility", menuName = "New Mobility Option")]
public class Mobility_Attributes : ScriptableObject
{
    public float spdIncrease;
    public float duration;
    public float coolDown;
    public bool canSteer;
    public bool canPhase;
}
