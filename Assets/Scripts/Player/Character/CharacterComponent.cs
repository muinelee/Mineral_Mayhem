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
        Character.OnHitEvent += OnHit;
        Character.OnHealEvent += OnHeal;
        Character.OnBlockEvent += OnBlock;
        Character.OnStatusBeginEvent += OnStatusBegin;
        Character.OnStatusEndedEvent += OnStatusEnded;
        Character.OnCleanseEvent += OnCleanse;
        Character.OnPickupEvent += OnPickup;
        Character.OnEnergyChangeEvent += OnEnergyChange;
        Character.OnCharacterDeathEvent += OnCharacterDeath;
        Character.OnRoundEndEvent += OnRoundEnd;
    }

    /// <summary>
    /// Called when a player takes damage
    /// </summary>
    public virtual void OnHit(float x, bool hitReact) { }

    /// <summary>
    /// Called when a player gets healed
    /// </summary>
    public virtual void OnHeal(float x) { }    
    
    /// <summary>
    /// Called when a player starts to block if true or stops blocking if false
    /// </summary>
    public virtual void OnBlock(bool isBlocking) { }

    /// <summary>
    /// Called when a player gets afflicted (Parameters need to be changed when statuses will be implemented, most likely)
    /// </summary>
    public virtual void OnStatusBegin(StatusEffect status) { }

    /// <summary>
    /// Called when a player affliction ends (Parameters need to be changed when statuses will be implemented, most likely)
    /// </summary>
    public virtual void OnStatusEnded(StatusEffect status) { }

    /// <summary>
    /// Called when a player cleanses debuffs
    /// </summary>
    public virtual void OnCleanse() { }

    /// <summary>
    /// Called when a player dies
    /// </summary>
    public virtual void OnCharacterDeath() { }

    /// <summary>
    /// Called when the round is complete
    /// </summary>
    public virtual void OnRoundEnd() { }

    /// <summary>
    /// Called when a character picks something up
    /// </summary>
    public virtual void OnPickup(bool isSpeed) { }

    public virtual void OnEnergyChange(float x) { }
}
