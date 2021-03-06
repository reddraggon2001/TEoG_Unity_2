﻿using UnityEngine;

public enum ModTypes
{
    Flat = 100,
    Precent = 200,
}

[System.Serializable]
public class StatMod : Mod
{
    public StatMod(float parValue, string parSource, ModTypes parType) : base(parValue, parType, parSource)
    {
    }
}

[System.Serializable]
public class AssingStatmod
{
    [SerializeField] private StatMod statMod;
    [SerializeField] private StatTypes statTypes;

    public AssingStatmod(StatMod statMod, StatTypes statTypes)
    {
        this.statMod = statMod;
        this.statTypes = statTypes;
    }

    public StatMod StatMod => statMod;
    public StatTypes StatTypes => statTypes;
}