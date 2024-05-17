using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLoadLayout : MonoBehaviour
{
    public SO_ArenaDefinition definition;

    private void Start()
    {
        Assert.Check(definition);
        GameManager.LoadLayout(definition.buildIndex);
    }
}
