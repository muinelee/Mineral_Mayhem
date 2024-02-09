using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ReadyUI : NetworkBehaviour
{
    [SerializeField] private GameObject hostUI;

    // Start is called before the first frame update
    void Start()
    {
        if (Object.HasStateAuthority) hostUI.SetActive(true);
    }
}