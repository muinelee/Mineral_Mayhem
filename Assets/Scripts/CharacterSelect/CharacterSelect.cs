using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : NetworkBehaviour
{
    // Instance
    public static CharacterSelect instance;

    [Header("Character Select")]
    public List<SO_Character> characters;

    [Header("UI Elements")]
    [SerializeField] private GameObject characterSelectScreen;
    [SerializeField] private Button[] characterButtons;
    [SerializeField] private Button[] abilityPortraits;
    [SerializeField] private TMP_Text currentAbilityDescription;
    [SerializeField] private TMP_Text backstory;
    private Button currentSelectedCharacterButton;
    private Button currentSelectedAbilityButton;

    // Can decouple into the Arena Manager script
    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    private CharacterEntity currentCharacterInstance;

    // Start is called before the first frame update
    private void Start()
    {
        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i;
            characterButtons[i].onClick.AddListener(() => SelectCharacter(characters[index], characterButtons[index]));
        }

        instance = this;
    }

    private void SelectCharacter (SO_Character character, Button selectedButton)
    {
        if (!Object.HasStateAuthority) RPC_SpawnCharacter(character.prefab, Object.InputAuthority);
        /*
        if (Runner.IsServer)
        {
            if (currentCharacterInstance != null)
            {
                Runner.Despawn(currentCharacterInstance.GetComponent<NetworkObject>());
            }

            // Rplace instantiate with Spawn    Instantiate(character.prefab, spawnPoints[0].position, spawnPoints[0].rotation);        
            currentCharacterInstance = Runner.Spawn(character.prefab, spawnPoints[0].position, spawnPoints[0].rotation, Object.InputAuthority);
        }
        */

        // Update character backstory text
        backstory.text = character.backstory;

        // Update UI for selected character button
        if (currentSelectedCharacterButton != null)
        {
            // Reset the previous selected button to its normal state
            ResetButtonVisual(currentSelectedCharacterButton);
        }

        // Update the current selection and its visual state
        currentSelectedCharacterButton = selectedButton;
        SetButtonAsSelected(currentSelectedCharacterButton);

        // Setup ability portraits and descriptions
        SetupAbilityUI(character);
        UpdateAbilityDescription(character.characterBasicAbilityDescription);
    }

    public void SpawnCharacter(CharacterEntity character, PlayerRef player)
    {
        if (!Runner.IsServer) return;

        Runner.Spawn(character, spawnPoints[0].position, spawnPoints[0].rotation, player);
    }

    private void SetupAbilityUI(SO_Character character)
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

    private void SetButtonAsSelected(Button button)
    {
        button.interactable = false;
    }

    private void ResetButtonVisual(Button button)
    {
        button.interactable = true;
    }

    private void UpdateAbilityDescription(string description)
    {
        currentAbilityDescription.text = description;
    }

    // TODO: Lock In Character
    public void LockInCharacter()
    {
        // Implement logic to lock in character, disable character selection UI
    }

    public void ActivateCharacterSelect()
    {
        characterSelectScreen.SetActive(true);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SpawnCharacter(CharacterEntity character, PlayerRef player)
    {
        CharacterSelect.instance.SpawnCharacter(character, player);
    }
}