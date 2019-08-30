﻿using System.Collections.Generic;
using UnityEngine;
public enum Genders
{
    Male,
    Female,
    Herm,
    Dickgirl,
    Cuntboy,
    Doll
}
public enum GenderType
{
    Feminine,
    Masculine
}
public abstract class BasicChar : MonoBehaviour
{
    public CharSprites sprites;
    public SpriteRenderer spriteRenderer;
    [Header("Scriptable objects")]
    public Settings settings;

    public EventLog eventLog;

    [Space]
    public string firstName, lastName;

    public string FullName { get { return $"{firstName} {lastName}"; } }

    public Inventory Inventory;
    public RaceSystem raceSystem = new RaceSystem();
    public string Race { get { return raceSystem.CurrentRace().ToString(); } }
    public Genders Gender
    {
        get
        {
            if (Dicks.Count > 0 && Vaginas.Count > 0)
            {
                return Genders.Herm;
            }else if (Dicks.Count > 0 && Boobs.Total() > 2)
            {
                return Genders.Dickgirl;
            }
            else if (Dicks.Count > 0)
            {
                return Genders.Male;
            }
            else if (Vaginas.Count > 0 && Boobs.Total() > 2)
            {
                return Genders.Female;
            }
            else if (Vaginas.Count > 0)
            {
                return Genders.Cuntboy;
            }
            else
            {
                return Genders.Doll;
            }
        }
    }
    public GenderType GenderType
    {
        get
        {
            switch (Gender)
            {
                case Genders.Cuntboy:
                case Genders.Doll:
                case Genders.Male:
                    return GenderType.Masculine;
                case Genders.Dickgirl:
                case Genders.Female:
                case Genders.Herm:
                default:
                    return GenderType.Feminine;
            }
        }
    }
    public Looks Looks;
    public VoreEngine Vore;
    public Age Age;
    public Body Body;
    public float Weight { get { return Body.weight.Value; } set { Body.weight.Value = value; } }

    public virtual void Awake()
    {
        Looks = new Looks(settings, this);
        Vore = new VoreEngine(eventLog, this);
        Age = new Age();
        Inventory.Owner = this;
    }

    public Health HP;
    public Health WP;

    [Header("Level,exp, stats & perks")]
    [SerializeField]
    public ExpSystem expSystem = new ExpSystem();

    public int Level { get { return expSystem.Level; } }
    public int Exp { get { return expSystem.Exp; } set { expSystem.Exp += value; } }
    public int StatsPoints { get { return expSystem.StatPoints; } }
    public bool StatBool { get { return expSystem.StatBool(); } }
    public int PerkPoints { get { return expSystem.PerkPoints; } }
    public bool PerkBool { get { return expSystem.PerkBool(); } }

    [SerializeField]
    public Perks Perk = new Perks();

    [Header("Stats")]
    public CharStats strength;
    public CharStats charm;
    public CharStats endurance;
    public CharStats dexterity;
    public CharStats intelligence;
    public void init(int lvl, float maxhp, float maxwp)
    {
        HP = new Health(maxhp);
        WP = new Health(maxwp);
        expSystem.Level = lvl;
    }
    [Header("Essence")]
    [SerializeField]
    private bool autoEss = true;

    public bool AutoEss { get { return autoEss; } }

    public void ToggleAutoEssence()
    {
        autoEss = autoEss ? false : true;
        // if autoessence check if need to grow stuff
    }

    [SerializeField]
    private Essence masc = new Essence();
    [SerializeField]
    private Essence femi = new Essence();

    public Essence Masc { get { return masc; } }
    public Essence Femi { get { return femi; } }
    public float EssDrain { get { return 3 + Perk.PerkBonus(PerksTypes.GainEss); } }
    public float EssGive { get { return 3 + Perk.PerkBonus(PerksTypes.GiveEss); } }
    public float RestRate { get { return 1f + Perk.PerkBonus(PerksTypes.FasterRest); } }

    public float LoseMasc(float mascToLose)
    {
        float have = Masc.Lose(mascToLose);
        float missing = mascToLose - have;
        if (missing > 0)
        {
            float fromOrgans = 0f;
            while (missing > fromOrgans && (dicks.Count > 0 || balls.Count > 0))// have needed organs
            {
                if (balls.Count > 0 && dicks.Count > 0)
                {
                    if (Dicks.Total() >= Balls.Total() * 2f + 1f)
                    {
                        fromOrgans += Dicks.ReCycle();
                    }
                    else
                    {
                        fromOrgans += Balls.ReCycle();
                    }
                }
                else if (balls.Count > 0)
                {
                    fromOrgans += Balls.ReCycle();
                }
                else
                {
                    fromOrgans += Dicks.ReCycle();
                }
            }
            have += Mathf.Min(fromOrgans, missing);
            float left = fromOrgans - missing;
            if (left > 0)
            {
                Masc.Gain(left);
            }
        }
        return have;
    }

    public float LoseFemi(float femiToLose)
    {
        float have = Femi.Lose(femiToLose);
        float missing = femiToLose - have;
        if (missing > 0)
        {
            float fromOrgans = 0f;
            while (missing > fromOrgans && (Vaginas.Count > 0 || Boobs.Count > 0))// have needed organs
            {
                if (Boobs.Count > 0 && Vaginas.Count > 0)
                {
                    if (Boobs.Total() >= Vaginas.Total() * 2f + 1f)
                    {
                        fromOrgans += Boobs.ReCycle();
                    }
                    else
                    {
                        fromOrgans += Vaginas.ReCycle();
                    }
                }
                else if (Vaginas.Count > 0)
                {
                    fromOrgans += Vaginas.ReCycle();
                }
                else
                {
                    fromOrgans += Boobs.ReCycle();
                }
            }
            have += Mathf.Min(fromOrgans, missing);
            float left = fromOrgans - missing;
            if (left > 0)
            {
                Femi.Gain(left);
            }
        }
        return have;
    }
    [Space]
    [SerializeField]
    private float gold = 0;

    public float Gold
    {
        get { return Mathf.Floor(gold); }
        set
        {
            gold += Mathf.Clamp(value, -gold, Mathf.Infinity);
        }
    }

    public bool CanAfford(int cost)
    {
        if (cost <= Gold)
        {
            Gold -= cost;
            return true;
        }
        else
        {
            return false;
        }
    }

    public Flags Flags;

    [Header("Organs")]
    [SerializeField]
    private List<Dick> dicks = new List<Dick>();

    public List<Dick> Dicks { get { return dicks; } }

    [SerializeField]
    private List<Balls> balls = new List<Balls>();

    public List<Balls> Balls { get { return balls; } }

    public float CumSlider()
    {
        return Balls.CumTotal() / Balls.CumMax();
    }

    public string CumStatus()
    {
        return $"{Mathf.Round(Balls.CumTotal())}";
    }

    [SerializeField]
    private List<Boobs> boobs = new List<Boobs>();

    public List<Boobs> Boobs { get { return boobs; } }

    [SerializeField]
    private bool lactating = false;

    public bool Lactating { get { return lactating; } }

    public float MilkSlider()
    {
        return Boobs.MilkTotal() / Boobs.MilkMax();
    }

    public string MilkStatus()
    {
        return $"{Mathf.Round(Boobs.MilkTotal() / 1000)}";
    }

    [SerializeField]
    private List<Vagina> vaginas = new List<Vagina>();

    public List<Vagina> Vaginas { get { return vaginas; } }
    [SerializeField]
    public SexStats sexStats = new SexStats();

    private void Start()
    {
        spriteRenderer.sprite = sprites.GetSprite(this);
    }
    private void Update()
    {
        RefreshOrgans();
        spriteRenderer.sprite = sprites.GetSprite(this);
    }
    public void RefreshOrgans()
    {
        foreach (Dick d in Dicks.FindAll(x => x.Size <= 0))
        {
            Dicks.Remove(d);
        }
        foreach (Balls b in balls.FindAll(x => x.Size <= 0))
        {
            balls.Remove(b);
        }
        foreach (Vagina v in vaginas.FindAll(x => x.Size <= 0))
        {
            vaginas.Remove(v);
        }
        foreach (Boobs b in boobs.FindAll(x => x.Size <= 0))
        {
            boobs.Remove(b);
        }
        if (autoEss)
        {
            if (Masc.Amount > 0)
            {
                if (Dicks.Total() <= Balls.Total() * 2f + 1f)
                {
                    if (dicks.Exists(d => Masc.Amount >= d.Cost))
                    {
                        foreach (Dick d in dicks)
                        {
                            if (Masc.Amount >= d.Cost)
                            {
                                Masc.Lose(d.Grow());
                            }
                        }
                    }
                    else if (Masc.Amount >= Dicks.Cost())
                    {
                        Masc.Lose(Dicks.Cost());
                        Dicks.AddDick();
                    }
                }
                else
                {
                    if (balls.Exists(b => Masc.Amount >= b.Cost))
                    {
                        foreach (Balls b in balls)
                        {
                            if (Masc.Amount >= b.Cost)
                            {
                                Masc.Lose(b.Grow());
                            }
                        }
                    }
                    else if (Masc.Amount >= Balls.Cost())
                    {
                        masc.Lose(Balls.Cost());
                        Balls.AddBalls();
                    }
                }
            }
            if (Femi.Amount > 0)
            {
                if (Boobs.Total() <= Vaginas.Total() * 1.5f + 1f)
                {
                    if (Boobs.Exists(b => Femi.Amount >= b.Cost))
                    {
                        foreach (Boobs b in boobs)
                        {
                            if (Femi.Amount >= b.Cost)
                            {
                                Femi.Lose(b.Grow());
                            }
                        }
                    }
                    else if (Femi.Amount >= Boobs.Cost())
                    {
                        Femi.Lose(Boobs.Cost());
                        Boobs.AddBoobs();
                    }
                }
                else
                {
                    if (Vaginas.Exists(v => Femi.Amount >= v.Cost))
                    {
                        foreach (Vagina v in vaginas)
                        {
                            if (Femi.Amount >= v.Cost)
                            {
                                Femi.Lose(v.Grow());
                            }
                        }
                    }
                    else if (Femi.Amount >= Vaginas.Cost())
                    {
                        Femi.Lose(Vaginas.Cost());
                        Vaginas.AddVag();
                    }
                }
            }
        }
    }
}