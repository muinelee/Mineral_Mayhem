using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterComponent : NetworkBehaviour
{
    public CharacterEntity Character { get; private set; }

    public virtual void Init(CharacterEntity character)
    {
        Character = character;
    }

    //  Can implement any other virtual functions we might need the character components to have
}
