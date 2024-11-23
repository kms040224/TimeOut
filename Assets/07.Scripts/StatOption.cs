using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatOption
{
    public string statName;
    public float upgradeValue;

    public StatOption(string statName, float upgradeValue)
    {
        this.statName = statName;
        this.upgradeValue = upgradeValue;
    }
}