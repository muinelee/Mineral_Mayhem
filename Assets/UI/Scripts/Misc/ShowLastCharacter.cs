using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowLastCharacter : MonoBehaviour
{
    [SerializeField] GameObject[] characterObjects;

    //----------------------------------------//

    private void Start()
    {
        characterObjects[ClientInfo.CharacterID].SetActive(true);
    }
}
