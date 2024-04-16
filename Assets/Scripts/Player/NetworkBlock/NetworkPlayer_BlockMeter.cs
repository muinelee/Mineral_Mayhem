using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer_BlockMeter : CharacterComponent
{
    /*
        Block Meter will be the resource for the block in the NetworkPlayer_Block script
        Block Meter increases over time if player is not block or it has been depleted
        If the block meter has been depleated, the player will be stunned, and will not be able to block until it is full
    */

    // Shield Meter properties
    [SerializeField] private float maxBlockMeter = 100;
    private float currentBlockMeter;

    public override void Init(CharacterEntity character)
    {
        base.Init(character);
        //TODO: character.SetBlock(this);
        
        currentBlockMeter = maxBlockMeter;
    }

    public override void FixedUpdateNetwork()
    {
        ManageBlockGain();
    }

    private void ManageBlockGain()
    {
        if (currentBlockMeter == maxBlockMeter)
        {
            //TODO: canBlock = true;
        }
        
        else if (currentBlockMeter < maxBlockMeter) currentBlockMeter += Runner.DeltaTime;
        else currentBlockMeter = maxBlockMeter;
    }

    public void RefillBlockMeter(float value)
    {
        // TODO:
/*        if (!isBlocking)
        {
            currentBlockMeter += value / 100 * maxBlockMeter;        
        }*/
    }

    public float GetBlockPercentage()
    {
        return currentBlockMeter / maxBlockMeter;
    }

    public bool IsBlockMeterFull()
    {
        if (currentBlockMeter == maxBlockMeter)
        {
            return true;
        }
        else return false;
    }
}
