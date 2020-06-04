using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat 
{
    [SerializeField]
    private BarScript bar;

    [SerializeField]
    private float maxVal;

    [SerializeField]
    private float currentVal;

    public float CurrentVal
    {
        get
        {
            return currentVal;
        }
        set
        {
            currentVal = Mathf.Clamp(value, 0, MaxVal);
            Bar.Value = currentVal;
        }
    }

    public float MaxVal 
    { 
        get
        {
            return maxVal;
        }
        set
        {
            maxVal = value;
            Bar.MaxValue = maxVal;
        } 
    }

    public BarScript Bar
    {
        get
        {
            return bar;
        }
    }

    public void Initialize()
    {
        this.MaxVal = maxVal;
        this.CurrentVal = currentVal;
    }
}
