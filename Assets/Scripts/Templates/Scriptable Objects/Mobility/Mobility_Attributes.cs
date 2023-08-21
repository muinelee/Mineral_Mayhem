using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mobility Option", menuName = "Mobility_Attributes")]
public class Mobility_Attributes : ScriptableObject
{
    public float spdIncrease;
    public float duration;
    public float coolDown;
    public bool canSteer;
    public bool isInvicible;

    public bool isActive;
    public bool canDash;

    public void Activate()
    {
        if (canDash)
        {
            isActive = true;
            canDash = false;
        }
    }
}
