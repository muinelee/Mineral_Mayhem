using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseStateOnLoad : MonoBehaviour
{
    [SerializeField] private bool visibleOnStart;

    //---------------------------------------//

    private void Start()
    {
        if (visibleOnStart)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
