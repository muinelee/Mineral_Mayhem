using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusHandler : CharacterComponent
{
    List<StatusData> statuses = new List<StatusData>();

    public Stat health;
    public Stat speed;
    public int stun = 0;

    private void Start()
    {
        speed.OnStatChanged += SpeedStatChangedCallback;
    }

    private void OnDisable()
    {
        speed.OnStatChanged -= SpeedStatChangedCallback;
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
                data.timeUntilNextTick = data.status.tickRate;
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

    public bool CheckIfStunned()
    {
        if (statuses.Contains(statuses.Find(x => x.status is StunStatus)))
        {
            return true;
        }
        else
        {
            return false;
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
        Debug.Log("Stun Status Applied");
    }

    public void RemoveStatus(StatusData data)
    {
        //data.status.OnStatusEnded(this);
        statuses.Remove(data);
    }

    private void SpeedStatChangedCallback()
    {
        NetworkPlayer_Movement networkPlayer_Movement = GetComponent<NetworkPlayer_Movement>();
        if (networkPlayer_Movement != null)
        {
            //networkPlayer_Movement.SetSpeed(speed.GetValue());
        }
    }
}
