using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [Header("Character Select")]
    public List<SO_Character> characters;

    [Header("UI Elements")]
    public Button[] characterButtons;

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

        // Future implementation for updating UI elements for character portrait, abilities images and descriptions, etc.
    }

    // TODO: Lock In Character
    public void LockInCharacter()
    {
        // Implement logic to lock in character, disable character selection UI
    }
}
