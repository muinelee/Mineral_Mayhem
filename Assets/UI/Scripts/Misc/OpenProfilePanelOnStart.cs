using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenProfilePanelOnStart : MonoBehaviour
{
    [SerializeField] CG_Fade[] panelsToOpen;
    [SerializeField] CG_Fade[] panelsToClose;

    //----------------------------------------------//

    private void Start()
    {
        if (!PlayerPrefs.HasKey("FirstLoad"))
        {
            PlayerPrefs.SetInt("FirstLoad", 1);
            for (int i = 0; i < panelsToOpen.Length; i++)
            {
                panelsToOpen[i].gameObject.SetActive(true);
                panelsToOpen[i].FadeIn();
            }
            for (int i = 0; i < panelsToClose.Length; i++)
            {
                panelsToClose[i].gameObject.SetActive(false);
            }
        }
    }
}
