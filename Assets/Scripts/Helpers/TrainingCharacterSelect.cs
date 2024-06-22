using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingCharacterSelect : MonoBehaviour
{
    private CharacterSelect characterSelect;
    // Start is called before the first frame update
    void Start()
    {
        characterSelect = FindAnyObjectByType<CharacterSelect>();
    }

    private void Update()
    {
        if (NetworkPlayer.Local == null)
        {
            Debug.Log("There is no Local Player");
            return;
        }

        if (!characterSelect.characterLookup.ContainsKey(NetworkPlayer.Local))
        {
            characterSelect.characterLookup.Add(NetworkPlayer.Local, null);
            return;
        }

        if (characterSelect.characterLookup[NetworkPlayer.Local] != null)
        {
            Debug.Log("Character look up has a character entity");
            return;
        }

        characterSelect.SelectCharacter(ClientInfo.CharacterID);
        this.gameObject.SetActive(false);
    }
}
