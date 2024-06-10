using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterSelect : NetworkBehaviour
{
    // Character select event for storm mechanics
    public delegate void CharacterSelectEvent();

    public static CharacterSelect instance;

    [Header("Character Select")]
    public List<SO_Character> characters;
    public Dictionary<NetworkPlayer, CharacterEntity> characterLookup = new Dictionary<NetworkPlayer, CharacterEntity>();

    [Header("UI Elements")]
    [SerializeField] private CG_Fade characterSelectScreen;
    [SerializeField] private BTN_OpenClose[] abilityPortraits;
    [SerializeField] private TMP_Text currentAbilityDescription;
    [SerializeField] private TMP_Text backstory;

    [SerializeField] private Slider sliderHealth;
    [SerializeField] private Slider sliderDamage;
    [SerializeField] private Slider sliderSpeed;

    [Header("Level start")]
    [SerializeField] private float characterSelectDuration = 10;
    private TickTimer characterSelectTimer = TickTimer.None;

    // Can decouple into the Arena Manager script
    [Header("Spawn Points")]
    public Transform[] spawnPoints;
    private int spawnPoint;

    [Header("Match Start Points")]
    public Transform[] matchStartPoints;

    //public event for storm mechanics
    public static event CharacterSelectEvent OnCharacterSelect;
    public UnityEvent finalizeCharSelect;

    private void Awake()
    {
        instance = this;
    }

    private void FixedUpdate()
    {
        if (!characterSelectTimer.Expired(Runner)) return;

        characterSelectTimer = TickTimer.None;
        if (!characterLookup[NetworkPlayer.Local].Input.CharacterSelected) FinalizeChoice();

        NetworkCameraEffectsManager.instance.StartCinematic(NetworkPlayer.Local);
        if (RoundManager.Instance) RoundManager.Instance.MatchStart();
        this.gameObject.SetActive(false);
    }

    public void SelectCharacter(int characterIndex)
    {
        int playerID = NetworkPlayer.Local.Object.InputAuthority.PlayerId;
        NetworkPlayer.Local.RPC_SetCharacterID(characterIndex);

        // Play voice line for selected character
        AudioManager.Instance.PlayAudioSFX(characters[characterIndex].voiceLine[0], spawnPoints[spawnPoint].position);

        RPC_SpawnCharacter(playerID, spawnPoint);

        ClientInfo.CharacterID = characterIndex;
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

        Sprite[] abilityPortraitsIcons =
        {
            character.characterBasicAbilityPortraitIcon,
            character.characterQAbilityPortraitIcon,
            character.characterEAbilityPortraitIcon,
            character.characterFAbilityPortraitIcon
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
            this.abilityPortraits[i].transform.GetChild(0).GetComponent<Image>().sprite = abilityPortraitsIcons[i];
            int index = i;
            this.abilityPortraits[i].onPress.RemoveAllListeners();
            this.abilityPortraits[i].onPress.AddListener(() => UpdateAbilityDescription(abilityDescriptions[index]));
        }
    }

    private void SetupStatbars(SO_Character character)
    {
        sliderHealth.value = character.health;
        sliderDamage.value = character.damage;
        sliderSpeed.value = character.speed;
    }

    private void UpdateAbilityDescription(string description)
    {
        currentAbilityDescription.text = description;
    }

    public void ActivateCharacterSelect()
    {
        //  characterSelectScreen.gameObject.SetActive(true);
        //  characterSelectScreen.FadeIn();

        /*
        RoundManager.Instance.ResetRound += SetPlayerToSpawn;
        foreach (NetworkPlayer player in NetworkPlayer.Players)
        {
            int spawnLocation = (player.team == NetworkPlayer.Team.Red) ? 0 : 2;
            spawnLocation += ReadyUpManager.instance.GetIndex(player);
            Vector3 spawnVector = spawnPoints[spawnLocation].position;
            RoundManager.Instance.respawnPoints.Add(player, spawnVector);
        }

        // Set camera location
        spawnPoint = (NetworkPlayer.Local.team == NetworkPlayer.Team.Red) ? 0 : 2;
        spawnPoint += ReadyUpManager.instance.GetIndex(NetworkPlayer.Local);
        if (NetworkPlayer.Local.team == NetworkPlayer.Team.Red) NetworkCameraEffectsManager.instance.GoToRedCamera();
        else NetworkCameraEffectsManager.instance.GoToBlueCamera();

        // Character Select Timer
        characterSelectTimer = TickTimer.CreateFromSeconds(Runner, characterSelectDuration);
        */

        //invoke the event for the storm
        //OnCharacterSelect?.Invoke();
        StartCoroutine(SpawnPrevCharacter());
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SpawnCharacter(int playerID, int spawnLocation)
    {
        if (!Runner.IsServer) return;

        int playerIndex = 0;

        foreach (NetworkPlayer p in NetworkPlayer.Players)
        {
            if (p.Object.InputAuthority.PlayerId == playerID)
            {
                playerIndex = NetworkPlayer.Players.IndexOf(p);
            }
        }

        NetworkPlayer player = NetworkPlayer.Players[playerIndex];

        if (characterLookup.ContainsKey(player) == false) characterLookup.Add(player, null);

        if (characterLookup[player] == null)
        {
            characterLookup[player] = Runner.Spawn(characters[player.CharacterID].prefab, spawnPoints[spawnLocation].position, Quaternion.identity, player.Object.InputAuthority);
            player.Avatar = characterLookup[player].Input;
        }

        else
        {
            Runner.Despawn(characterLookup[player].Object);
            characterLookup[player] = Runner.Spawn(characters[player.CharacterID].prefab, spawnPoints[spawnLocation].position, Quaternion.identity, player.Object.InputAuthority);
            player.Avatar = characterLookup[player].Input;
        }

        characterLookup[player].GetComponent<NetworkPlayer_Health>().team = player.team;

        RPC_UpdateCharacterLookupForClients(player, characterLookup[player]);
    }

    /// <summary>
    /// Syncs CharacterLookup  across all clients from the original RPC_SpawnCharacter call
    /// </summary>
    /// <param name="player"></param>
    /// <param name="character"></param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateCharacterLookupForClients(NetworkPlayer player, CharacterEntity character)
    {
        if (character != null)
        {
            characterLookup[player] = character;
            SO_Character characterSO = characters[NetworkPlayer.Local.CharacterID];

            // Update character backstory text
            backstory.text = characterSO.backstory;

            // Setup ability portraits and descriptions
            SetupAbilityUI(characterSO);
            UpdateAbilityDescription(characterSO.characterBasicAbilityDescription);
            SetupStatbars(characterSO);
        }
        else characterLookup.Remove(player);
    }

    /// <summary>
    /// Designed for the Character Select button, to finalize your character selection
    /// </summary>
    public void FinalizeChoice()
    {
        if (!this.gameObject.activeSelf) return;

        // Enable player control
        characterLookup[NetworkPlayer.Local].Input.CharacterSelected = true;
        characterLookup[NetworkPlayer.Local].PlayerUI.SpawnPlayerUI();
        NetworkPlayer_InGameUI.instance.ShowPlayerUI();
        

        NetworkCameraEffectsManager.instance.GoToTopCamera();
        //  ResetButtonVisual(currentSelectedCharacterButton);
        //  characterSelectScreen.gameObject.SetActive(true);
        //  characterSelectScreen.FadeOut();
        OnCharacterSelect?.Invoke();
        finalizeCharSelect?.Invoke();
    }

    /// <summary>
    /// This feature will be for when we want to test different characters before we begin the first round, to re-enable character select
    /// </summary>
    public void RenableCharacterSelect()
    {
        characterSelectScreen.gameObject.SetActive(true);
        characterSelectScreen.FadeIn();
        //  reselectButton.gameObject.SetActive(false);

        //RPC_CharacterReselect(NetworkPlayer.Local);
        if (NetworkPlayer.Local.team == NetworkPlayer.Team.Red) NetworkCameraEffectsManager.instance.GoToRedCamera();
        else if (NetworkPlayer.Local.team == NetworkPlayer.Team.Blue) NetworkCameraEffectsManager.instance.GoToBlueCamera();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_CharacterReselect(NetworkPlayer player)
    {
        Runner.Despawn(characterLookup[player].Object);
        RPC_UpdateCharacterLookupForClients(player, null);
    }

    private void SetPlayerToSpawn()
    {
        if (RoundManager.Instance)
        {
            foreach (NetworkPlayer player in NetworkPlayer.Players)
            {
                Vector3 spawnPos = RoundManager.Instance.respawnPoints[player];
                characterLookup[player].gameObject.transform.position = spawnPos;
            }
        }
        else
        {
            characterLookup[NetworkPlayer.Local].gameObject.transform.position = Vector3.zero;
        }
    }

    IEnumerator SpawnPrevCharacter()
    {
        yield return 0;

        characterSelectScreen.gameObject.SetActive(true);
        characterSelectScreen.FadeIn();

        Debug.Log($"The size of the Players list is {NetworkPlayer.Players.Count}");

        RoundManager.Instance.ResetRound += SetPlayerToSpawn;
        foreach (NetworkPlayer player in NetworkPlayer.Players)
        {
            int spawnLocation = (player.team == NetworkPlayer.Team.Red) ? 0 : 2;

            spawnLocation += ReadyUpManager.instance.GetIndex(player);
            Debug.Log($"Spawn location value is {spawnLocation}");
            Vector3 spawnVector = matchStartPoints[spawnLocation].position;
            RoundManager.Instance.respawnPoints.Add(player, spawnVector);
        }

        // Set camera location
        spawnPoint = (NetworkPlayer.Local.team == NetworkPlayer.Team.Red) ? 0 : 2;
        spawnPoint += ReadyUpManager.instance.GetIndex(NetworkPlayer.Local);
        if (NetworkPlayer.Local.team == NetworkPlayer.Team.Red) NetworkCameraEffectsManager.instance.GoToRedCamera();
        else NetworkCameraEffectsManager.instance.GoToBlueCamera();

        // Character Select Timer
        characterSelectTimer = TickTimer.CreateFromSeconds(Runner, characterSelectDuration);

        SelectCharacter(ClientInfo.CharacterID);
    }
}