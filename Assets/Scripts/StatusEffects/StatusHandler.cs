using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusHandler : CharacterComponent
{
    List<StatusData> statuses = new List<StatusData>();

    public Stat health;
    public Stat armor;
    public Stat speed;
    public int stun = 0;

    public override void Init(CharacterEntity character)
    {
        base.Init(character);
        character.SetStatusHandler(this);
    }

    private void Update()
    {
        List<StatusData> statusesToRemove = new List<StatusData>();

        stun = 0;

        foreach (StatusData data in statuses)
        {
            data.timeUntilNextTick -= Time.deltaTime;
            if (data.timeUntilNextTick <= 0)
            {
                data.status.OnStatusUpdate(this);
                data.timeUntilNextTick += data.status.tickRate;
            }

            data.duration -= Time.deltaTime;
            if (data.duration <= 0)
            {
                statusesToRemove.Add(data);
            }
        }
        foreach(StatusData data in statusesToRemove)
        {
            RemoveStatus(data);

            if (Character) Character.OnStatusEnded(data.status);
            // Temporary solution for between 
            else data.status.OnStatusEnded(this);
        }
    }

    public override void OnStatusBegin(StatusEffect status)
    {
        AddStatus(status);
    }

    public override void OnStatusEnded(StatusEffect status)
    {
        status.OnStatusEnded(this);
    }

    public void AddStatus(StatusEffect effect)
    {
        StatusData data = new StatusData();
        data.status = effect;
        data.duration = effect.duration;
        data.timeUntilNextTick = effect.tickRate;

        statuses.Add(data);
        data.status.OnStatusApplied(this);
        if (effect is StunStatus) RPC_PlayEffectAnimation("Stunned");
    }

    public void RemoveStatus(StatusData data)
    {
        //data.status.OnStatusEnded(this);
        statuses.Remove(data);
    }

    public override void OnCleanse()
    {
        CleanseDebuff();
    }

    public void CleanseDebuff()
    {
        List<StatusData> statusesToRemove = new List<StatusData>();

        foreach (StatusData status in statuses)
        {
            if (status.status.isCleanseable)
            {
                statusesToRemove.Add(status);
            }
        }
        foreach (StatusData status in statusesToRemove)
        {
            status.status.OnStatusCleansed(this);
            statuses.Remove(status);
        }
    }
    
    public float GetArmorValue()
    {
        // As an example: Add 25 to armor value to reduce damage taken by 25%
        return Mathf.Clamp(armor.GetValue(), 0, 200);
    }

    public float GetDamageReduction()
    {
        return Mathf.Clamp(((200 - GetArmorValue()) / 100), 0, 2);
    }

    public bool IsStunned()
    {
        return stun > 0;
    }

    public bool HasUncleansableStun()
    {
        foreach (StatusData status in statuses)
        {
            if (status.status.GetType() == typeof(StunStatus) && !status.status.isCleanseable) return true;
        }
        return false;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayEffectAnimation(string effect)
    {
        this.Character.Animator.anim.CrossFade(effect, 0.1f);
    }
}
