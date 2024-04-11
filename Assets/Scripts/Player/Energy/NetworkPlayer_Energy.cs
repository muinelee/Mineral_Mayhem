using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer_Energy : CharacterComponent
{
    /*
    Energy will be the resource for the FAttack in the NetworkPlayer_Attack script
    Energy increases over time and is calculated as a percentage.
    Energy can be accessed by collectibles to call the addEnergy function
    */

    [SerializeField] private float fullCharge;
    private float energy;
    public override void Init(CharacterEntity character)
    {
        base.Init(character);
        character.SetEnergy(this);
    }

    public override void FixedUpdateNetwork()
    {
        ManageEnergyGain();
    }

    private void ManageEnergyGain()
    {
        if (energy == fullCharge) return;
        else if (energy < fullCharge) energy += Runner.DeltaTime;
        else energy = fullCharge;
    }

    public void AddEnergy(float value)
    {
        // Add value by percentage
        energy += value/100 * fullCharge;
    }

    public float GetEnergyPercentage()
    {
        return energy/fullCharge;
    }

    public bool IsUltCharged()
    {
        if (energy == fullCharge)
        {
            energy = 0;                         // if returns true, then the player is firing their ult. Reset the energy
            return true;
        }        
        else return false;
    }
}
