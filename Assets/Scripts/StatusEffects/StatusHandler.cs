using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusHandler : CharacterComponent
{
    List<StatusData> statuses = new List<StatusData>();

    public Stat health;
    public Stat speed;

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
                data.status.OnStatusEnded(this);
                RemoveStatus(data);
            }
        }
    }

    public void AddStatus(StatusEffect effect)
    {
        StatusData data = new StatusData();
        data.status = effect;
        data.duration = effect.duration;
        data.timeUntilNextTick = effect.tickRate;

        statuses.Add(data);
        data.status.OnStatusApplied(this);
    }

    public void RemoveStatus(StatusData data)
    {
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
