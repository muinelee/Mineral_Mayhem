using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                buttons[i].disabled = true;
            }
        }

        yield return new WaitForSecondsRealtime(cooldownTime);

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                buttons[i].disabled = false;
            }
        }
    }
}
