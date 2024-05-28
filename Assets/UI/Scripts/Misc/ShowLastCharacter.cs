using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowLastCharacter : MonoBehaviour
{
    [SerializeField] GameObject pyreObject;
    [SerializeField] GameObject crystaObject;
    [SerializeField] GameObject terranObject;

    //----------------------------------------//

    private void Start()
    {
        if (ClientInfo.CharacterID == 0)
        {
            crystaObject.SetActive(true);
        }
        else if (ClientInfo.CharacterID == 2)
        {
            pyreObject.SetActive(true);
        }
        else
        {
            terranObject.SetActive(true);
        }
    }
}
