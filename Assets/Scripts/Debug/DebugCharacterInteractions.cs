using UnityEngine;
using UnityEngine.UI;

public class DebugCharacterInteractions : MonoBehaviour
{
    [SerializeField] private Button HealButton;
    [SerializeField] private Button DmgButton;

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
}