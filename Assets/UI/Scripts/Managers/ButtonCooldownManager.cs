using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonCooldownManager : MonoBehaviour
{
    public static ButtonCooldownManager instance;

    [SerializeField] BTN_OpenClose[] buttons;

    [SerializeField] float cooldownTime = 1f;

    //--------------------------------------------//

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ButtonCooldown()
    {
        StartCoroutine(iButtonCooldown());
    }
    private IEnumerator iButtonCooldown()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                buttons[i].GetComponent<EventTrigger>().enabled = false;
            }
        }

        yield return new WaitForSecondsRealtime(cooldownTime);

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                buttons[i].GetComponent<EventTrigger>().enabled = true;
            }
        }
    }
}
