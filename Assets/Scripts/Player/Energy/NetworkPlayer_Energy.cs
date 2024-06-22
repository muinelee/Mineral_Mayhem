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
    [Networked] public float energy { get; set; }
    public override void Init(CharacterEntity character)
    {
        base.Init(character);
        character.SetEnergy(this);
    }

    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer) return;

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
        energy = Mathf.Min(energy + value/100 * fullCharge, fullCharge);
    }

    public float GetEnergyPercentage()
    {
        return energy/fullCharge;
    }

    public bool IsUltCharged()
    {
        return energy >= fullCharge;
    }

    public override void OnEnergyChange(float x)
    {
        energy += (x / 100 * fullCharge);
    }
}
