using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowLastCharacter : MonoBehaviour
{
    [SerializeField] GameObject[] characterObjects;
    [SerializeField] GameObject[] characterLights;

    //----------------------------------------//

    private void Start()
    {
        characterObjects[ClientInfo.CharacterID].SetActive(true);
        characterLights[ClientInfo.CharacterID].SetActive(true);
    }
}
