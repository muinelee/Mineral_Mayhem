using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class SO_Character : ScriptableObject
{
    public GameObject characterPrefab;
    public string characterName;
    public string backstory;
    public string characterRace;
    public Sprite characterBasicAbilityPortrait;
    public string characterBasicAbilityDescription;
    public Sprite characterQAbilityPortrait;
    public string characterQAbilityDescription;
    public Sprite characterEAbilityPortrait;
    public string characterEAbilityDescription;
    public Sprite characterFAbilityPortrait;
    public string characterFAbilityDescription;
}
