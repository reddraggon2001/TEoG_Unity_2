﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HealthTypes
{
    Health,
    WillPower
}

[System.Serializable]
public class Health : Stat
{
    [SerializeField] private float current;
    [SerializeField] private List<HealthMod> healthMods = new List<HealthMod>();
    [SerializeField] private List<TempHealthMod> tempHealthMods = new List<TempHealthMod>();
    [SerializeField] private Recovery recovery = new Recovery();
    public List<HealthMod> HealthMods => healthMods;

    public List<TempHealthMod> TempHealthMods => tempHealthMods;
    public Recovery Recovery => recovery;

    public override int Value
    {
        get
        {
            if (IsDirty)
            {
                lastValue = CalcValue;
                IsDirty = false;
                UpdateSliderEvent?.Invoke();
            }
            return lastValue;
        }
    }

    protected override bool IsDirty
    {
        get => isDirty;
        set
        {
            isDirty = value;
            if (value)
            {
                _ = Value;
            }
        }
    }

    public bool IsMax => current >= Value;

    private readonly List<AffectedByStat> AffectedBy = new List<AffectedByStat>();

    public void TickRecovery() => Gain(Recovery.Value);

    public void TickTempMods()
    {
        if (TempHealthMods.RemoveAll(tm => tm.Duration < 1) > 0)
        {
            AddedTempEvent?.Invoke();
            IsDirty = true;
        }
    }

    protected override int CalcValue
    {
        get
        {
            float flatValue = BaseValue +
                HealthMods.FindAll(hm => hm.ModType == ModTypes.Flat).Sum(hm => hm.Value) +
                TempHealthMods.FindAll(thm => thm.ModType == ModTypes.Flat).Sum(thm => thm.Value) +
                AffectedBy.Sum(ab => ab.CharStats.Value * ab.Multiplier);
            float perValue = 1 +
                HealthMods.FindAll(hm => hm.ModType == ModTypes.Precent).Sum(hm => hm.Value) +
                TempHealthMods.FindAll(thm => thm.ModType == ModTypes.Precent).Sum(thm => thm.Value);
            return Mathf.FloorToInt(flatValue * perValue);
        }
    }

    public Health(int parMax)
    {
        baseValue = parMax;
        current = parMax;
        DateSystem.NewHourEvent += TickTempMods;
    }

    public Health(int parMax, List<AffectedByStat> affectedBy) : this(parMax)
    {
        this.AffectedBy = affectedBy;
        this.AffectedBy.ForEach(ab =>
        {
            ab.CharStats.ValueChanged += () => IsDirty = true;
        });
    }

    public Health(int parMax, AffectedByStat affectedBy) : this(parMax, new List<AffectedByStat>() { affectedBy })
    {
    }

    public bool TakeDmg(float dmg)
    {
        current = Mathf.Max(0, current - dmg);
        UpdateSliderEvent?.Invoke();
        if (current <= 0)
        {
            DeadEvent?.Invoke();
            return true;
        }
        return false;
    }

    public void Gain(float gain)
    {
        current += Mathf.Clamp(gain, 0, Value - current);
        UpdateSliderEvent?.Invoke();
    }

    public void FullGain() => current = Value;

    public float SliderValue => current / Value;

    public string Status => $"{Mathf.FloorToInt(current)} / {Value}";

    public delegate void UpdateSlider();

    public event UpdateSlider UpdateSliderEvent;

    public delegate void Dead();

    public event Dead DeadEvent;

    public void ManualSliderUpdate() => UpdateSliderEvent?.Invoke();

    #region AddAndRemoveMods

    public void AddMods(HealthMod mod)
    {
        HealthMods.Add(mod);
        IsDirty = true;
    }

    public void AddTempMod(TempHealthMod mod)
    {
        if (TempHealthMods.Exists(tm => tm.Source.Equals(mod.Source)))
        {
            TempHealthMod toChange = TempHealthMods.Find(tm => tm.Source.Equals(mod.Source));
            float diminishingReturn = (float)toChange.Duration / (float)mod.Duration;
            int toIncrease = Mathf.Max(0, Mathf.FloorToInt(mod.Duration / Mathf.Max(1, 2 * diminishingReturn)));
            toChange.IncreaseDuration(toIncrease);
        }
        else
        {
            // Clone otherwise diminishingReturn doesn't work as duration increase on both.
            TempHealthMods.Add(new TempHealthMod(mod.Value, mod.ModType, mod.HealthType, mod.Source, mod.Duration));
        }
        IsDirty = true;
        AddedTempEvent?.Invoke();
    }

    public delegate void DelegateAddedTemp();

    public event DelegateAddedTemp AddedTempEvent;

    public void RemoveMods(HealthMod mod)
    {
        HealthMods.Remove(mod);
        IsDirty = true;
    }

    public void RemoveTempMods(TempHealthMod mod)
    {
        TempHealthMods.Remove(mod);
        IsDirty = true;
        AddedTempEvent?.Invoke();
    }

    public bool RemoveFromSource(string Source)
    {
        if (string.IsNullOrEmpty(Source))
        {
            return false;
        }
        if (HealthMods.Exists(sm => sm.Source.Equals(Source)))
        {
            foreach (HealthMod sm in HealthMods.FindAll(s => s.Source.Equals(Source)))
            {
                HealthMods.Remove(sm);
            }
            IsDirty = true;
            return true;
        }
        return false;
    }

    public bool RemoveTempFromSource(string Source)
    {
        if (string.IsNullOrEmpty(Source))
        {
            return false;
        }
        if (TempHealthMods.Exists(sm => sm.Source.Equals(Source)))
        {
            foreach (TempHealthMod sm in TempHealthMods.FindAll(s => s.Source.Equals(Source)))
            {
                TempHealthMods.Remove(sm);
            }
            IsDirty = true;
            AddedTempEvent?.Invoke();
            return true;
        }
        return false;
    }

    #endregion AddAndRemoveMods
}

public class AffectedByStat
{
    public AffectedByStat(CharStats charStats, float multiplier)
    {
        this.CharStats = charStats;
        this.Multiplier = multiplier;
    }

    public CharStats CharStats { get; }
    public float Multiplier { get; }
}