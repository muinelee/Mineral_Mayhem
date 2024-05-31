using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class INP_Pause : MonoBehaviour
{
    [SerializeField] KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] CG_Fade menu;
    [SerializeField] GameObject[] windowsToOpen;
    [SerializeField] GameObject[] windowsToClose;
    [SerializeField] float delay = 0.5f;

    public bool paused = false;

    // Used for ClosePauseMenuUntilPastCharacterSelect.cs
    public bool pastCharacterSelect = false;

    //--------------------------------//

    private void Update()
    {
        if (Input.GetKeyUp(pauseKey))
        {
            Pause();
        }
    }

    public void Pause()
    {
        StartCoroutine(iPause());
    }
    IEnumerator iPause()
    {
        if (paused)
        {
            menu.gameObject.SetActive(true);
            menu.FadeOut();

            yield return new WaitForSecondsRealtime(delay);

            paused = false;

            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }
        else if (!paused)
        {
            paused = true;

            menu.gameObject.SetActive(true);
            menu.FadeIn();

            if (windowsToClose.Length > 0)
            {
                for (int i = 0; i < windowsToClose.Length; i++)
                {
                    windowsToClose[i].GetComponent<CanvasGroup>().alpha = 0;
                    windowsToClose[i].gameObject.SetActive(false);
                }
            }
            if (windowsToOpen.Length > 0)
            {
                for (int i = 0; i < windowsToOpen.Length; i++)
                {
                    windowsToOpen[i].gameObject.SetActive(true);
                    windowsToOpen[i].GetComponent<CanvasGroup>().alpha = 1;
                }
            }

            //Cursor.lockState = CursorLockMode.Confined;
            //Cursor.visible = true;
        }
    }

    public void ChangePauseBool(bool isPaused)
    {
        paused = isPaused;
    }

    public void PastCharacterSelect()
    {
        pastCharacterSelect = true;
    }
}
