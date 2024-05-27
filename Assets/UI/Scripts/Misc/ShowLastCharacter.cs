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
        if (PlayerPrefs.GetInt("lastSelectedCharacter") == 0)
        {
            crystaObject.SetActive(true);
        }
        else if (PlayerPrefs.GetInt("lastSelectedCharacter") == 2)
        {
            pyreObject.SetActive(true);
        }
        else
        {
            terranObject.SetActive(true);
        }
    }
}
