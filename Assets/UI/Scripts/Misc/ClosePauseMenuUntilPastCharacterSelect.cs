using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePauseMenuUntilPastCharacterSelect : MonoBehaviour
{
    [SerializeField] INP_Pause pauseManager;
    
    //-------------------------------------------------------------//

    private void OnEnable()
    {
        if (pauseManager.pastCharacterSelect == false)
        {
            pauseManager.paused = false;
            gameObject.SetActive(false);
        }
    }
}
