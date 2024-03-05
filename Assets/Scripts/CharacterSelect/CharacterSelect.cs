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

    // Test
    public NetworkObject playerPF;
    public NetworkObject curPlayerObject;

    [Header("Character Select")]
    public List<SO_Character> characters;

    public Dictionary<NetworkPlayer, CharacterEntity> characterLookup = new Dictionary<NetworkPlayer, CharacterEntity>();

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
            characterButtons[index].onClick.AddListener(() => SelectCharacter(index, characterButtons[index]));
        }
        instance = this;
    }

    private void SelectCharacter (int characterIndex, Button selectedButton)
    {
        int index = NetworkPlayer.Players.IndexOf(NetworkPlayer.Local);
        NetworkPlayer.Local.RPC_SetCharacterID(characterIndex);

        // Needing for removing monobehaviour HUD before RPC call
        NetworkPlayer player = NetworkPlayer.Players[index];
        if (characterLookup.ContainsKey(player) == false) characterLookup.Add(player, null);
        Destroy(characterLookup[player].GetComponent<NetworkPlayer_OnSpawnUI>().playerUI.gameObject);

        RPC_SpawnCharacter(index);

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

        SO_Character character = characters[NetworkPlayer.Local.CharacterID];
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

    private void OnEnable()
    {
        foreach (NetworkPlayer player in NetworkPlayer.Players)
        {
            characterLookup.Add(player, null);
        }
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

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SpawnCharacter(int playerIndex)
    {
        if (!Runner.IsServer) return;

        NetworkPlayer player = NetworkPlayer.Players[playerIndex];

        if (characterLookup.ContainsKey(player) == false) characterLookup.Add(player, null);

        if (characterLookup[player] == null) characterLookup[player] = Runner.Spawn(characters[player.CharacterID].prefab, Vector3.zero, Quaternion.identity, player.Object.InputAuthority);

        else
        {
            //Temporary test fr desawning/destroying health bars
            //Runner.Despawn(characterLookup[player].GetComponent<NetworkPlayer_OnSpawnUI>().floatingHealthBar.Object);
            Runner.Despawn(characterLookup[player].Object);
            characterLookup[player] = Runner.Spawn(characters[player.CharacterID].prefab, Vector3.zero, Quaternion.identity, player.Object.InputAuthority);
        }
    }
}