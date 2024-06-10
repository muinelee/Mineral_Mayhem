using UnityEngine;
using UnityEngine.UI;

public class DebugCharacterInteractions : MonoBehaviour
{
    [SerializeField] private Button HealButton;
    [SerializeField] private Button DmgButton;
    [SerializeField] private Button EnergyButton;

    private void Start()
    {
        if (HealButton)
        {
            HealButton.onClick.AddListener(HealCharacter);
        }
        if (DmgButton)
        {
            DmgButton.onClick.AddListener(DamageCharacter);
        }
        if (EnergyButton)
        {
            EnergyButton.onClick.AddListener(EnergizeCharacter);
        }
    }

    public void HealCharacter()
    {
        CharacterEntity character = NetworkPlayer.Local.Avatar.Character;
        if (!character)
        {
            Debug.Log("No CharacterEntity found - get good.");
            return;
        }
        character.OnHeal(1f);
    }

    public void DamageCharacter()
    {
        CharacterEntity character = NetworkPlayer.Local.Avatar.Character;
        if (!character)
        {
            Debug.Log("No CharacterEntity found - get good.");
            return;
        }
        character.OnHit(1f, false);
    }

    public void EnergizeCharacter()
    {
        CharacterEntity character = NetworkPlayer.Local.Avatar.Character;
        if (!character)
        {
            Debug.Log("No CharacterEntity found - get good.");
            return;
        }
        character.Energy.AddEnergy(100f);
        character.OnPickup(false);
    }
}