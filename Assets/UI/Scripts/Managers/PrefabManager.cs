using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance;
    public Transform P_DamagePopup;

    //------------------------------------//

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
