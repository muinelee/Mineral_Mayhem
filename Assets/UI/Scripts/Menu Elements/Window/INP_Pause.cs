using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class INP_Pause : MonoBehaviour
{
    [SerializeField] KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] CG_Fade menu;
    [SerializeField] float delay = 0.5f;

    bool paused = false;

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

            //Cursor.lockState = CursorLockMode.Confined;
            //Cursor.visible = true;
        }
    }
}
