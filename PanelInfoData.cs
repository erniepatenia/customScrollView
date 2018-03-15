using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PanelInfoData : IComparable<PanelInfoData>
{
    public GameObject panelGO;
    public int value;
    public string name;
    public Color color = Color.blue;
    public BBG.Variables.FloatReference floatReference;

    public int CompareTo(PanelInfoData other)
    {
        if (other == null)
        {
            return 1;
        }

        //Return the difference in power.
        return value - other.value;
    }

    public PanelInfoData(int newValue)
    {
        value = newValue;
    }
}
