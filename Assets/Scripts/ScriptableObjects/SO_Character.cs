using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class SO_Character : ScriptableObject
{
    public GameObject characterPrefab;
    public string characterName;

    // Potential future fields
    // public string characterDescription;
    // public string backstory;
    // public string characterQAbility;
    // public string characterEAbility;
    // public string characterFAbility;
    // public image characterPortrait;
    // public image characterQAbility;
    // public image characterEAbility;
    // public image characterFAbility;
}
