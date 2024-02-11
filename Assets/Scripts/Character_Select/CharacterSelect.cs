using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [Header("Character Select")]
    public List<SO_Character> characters;

    [Header("UI Elements")]
    public Button[] characterButtons;
    public Button[] abilityPortraits;
    public TMP_Text currentAbilityDescription;
    public TMP_Text backstory;

    // Can decouple into the Arena Manager script
    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    private GameObject currentCharacterInstance;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i;
            characterButtons[i].onClick.AddListener(() => SelectCharacter(characters[index]));
        }
    }

    void SelectCharacter (SO_Character character)
    {
        if (currentCharacterInstance != null)
        {
            Destroy(currentCharacterInstance);
        }

        currentCharacterInstance = Instantiate(character.characterPrefab, spawnPoints[0].position, spawnPoints[0].rotation);
        
        backstory.text = character.backstory;

        // Setup ability portraits and descriptions
        SetupAbilityUI(character);
        UpdateAbilityDescription(character.characterBasicAbilityDescription);
    }

    void SetupAbilityUI(SO_Character character)
    {
        Sprite[] abilityPortraits =
        {
            character.characterBasicAbilityPortrait,
            character.characterQAbilityPortrait,
            character.characterEAbilityPortrait,
            character.characterFAbilityPortrait
        };

        string[] abilityDescriptions =
        {
            character.characterBasicAbilityDescription,
            character.characterQAbilityDescription,
            character.characterEAbilityDescription,
            character.characterFAbilityDescription
        };

        for (int i = 0; i < abilityPortraits.Length; i++)
        {
            this.abilityPortraits[i].GetComponent<Image>().sprite = abilityPortraits[i];
            int index = i;
            this.abilityPortraits[i].onClick.RemoveAllListeners();
            this.abilityPortraits[i].onClick.AddListener(() => UpdateAbilityDescription(abilityDescriptions[index]));
        }
    }

    void UpdateAbilityDescription(string description)
    {
        currentAbilityDescription.text = description;
    }

    // TODO: Lock In Character
    public void LockInCharacter()
    {
        // Implement logic to lock in character, disable character selection UI
    }
}
