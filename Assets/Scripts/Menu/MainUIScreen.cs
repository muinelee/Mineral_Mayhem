using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIScreen : MonoBehaviour
{
    private void Awake()
    {
        UIScreen.Focus(GetComponent<UIScreen>());
    }
}
