using System;
using UnityEngine;

[Serializable]
public class FloatReference
{ 
    public bool UseConstant;
    public float ConstantValue;
    public FloatVariable Variable;

    public float Value
    {
        get { return UseConstant ? ConstantValue : Variable.value; }
    }
}
