using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Stat
{
    // delegate for the event
    public event Action OnStatChanged;

    public float baseValue = 10.0f;

    public List<float> Modifiers = new List<float>();

    public void AddModifier(float amount)
    {
        Modifiers.Add(amount); 
        OnStatChanged?.Invoke();
    }

    public void RemoveModifier(float amount)
    {
        Modifiers.Remove(amount);
        OnStatChanged?.Invoke();
    }

    public float GetValue()
    {
        float finalValue = baseValue;

        foreach (float modifier in Modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }
}
