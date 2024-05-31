using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenProfilePanelOnStart : MonoBehaviour
{
    [SerializeField] CG_Fade panelProfile;

    //----------------------------------------------//

    private void Start()
    {
        if (SettingsManager.firstTimeAtMenu == true)
        {
            panelProfile.gameObject.SetActive(true);
            panelProfile.FadeIn();
            SettingsManager.firstTimeAtMenu = false;
        }
    }
}
