using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Scriptable Object/Character")]
public class SO_Character : ScriptableObject
{
    public CharacterEntity prefab;

    public AudioClip[] voiceLine;

    public Sprite characterBasicAbilityPortrait;
    public Sprite characterQAbilityPortrait;
    public Sprite characterEAbilityPortrait;
    public Sprite characterFAbilityPortrait;

    public string characterName;
    public string characterRace;

    [TextArea(3, 10)]
    public string backstory;
    [TextArea(3, 10)]
    public string characterBasicAbilityDescription;
    [TextArea(3, 10)]
    public string characterQAbilityDescription;
    [TextArea(3, 10)]
    public string characterEAbilityDescription;
    [TextArea(3, 10)]
    public string characterFAbilityDescription;
}
