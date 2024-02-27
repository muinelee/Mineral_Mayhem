using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterComponent : NetworkBehaviour
{
    public CharacterEntity Character { get; private set; }

    /// <summary>
    /// Called when the character entity is initialized
    /// </summary>
    public virtual void Init(CharacterEntity character)
    {
        Character = character;
    }

    /// <summary>
    /// Called when a player takes damage
    /// </summary>
    public virtual void OnHit(float x) { }

    /// <summary>
    /// Called when a player gets healed
    /// </summary>
    public virtual void OnHeal(float x) { }

    /// <summary>
    /// Called when a player gets afflicted (Parameters need to be changed when statuses will be implemented, most likely)
    /// </summary>
    public virtual void OnStatused() { }

    /// <summary>
    /// Called when a player dies
    /// </summary>
    public virtual void OnCharacterDeath() { }

    /// <summary>
    /// Called when the round is complete
    /// </summary>
    public virtual void OnRoundComplete() { }

    /// <summary>
    /// Called when a character picks something up
    /// </summary>
    public virtual void OnPickup() { }
}
