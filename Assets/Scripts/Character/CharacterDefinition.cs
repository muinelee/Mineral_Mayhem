using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Scriptable Object/Character Definition")]
public class CharacterDefinition : ScriptableObject
{
    public CharacterEntity prefab;

    // All other info pertaining to the character will be needed here as well, such as iconography, abilities, etc
}
