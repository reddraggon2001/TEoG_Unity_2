﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SexChar : MonoBehaviour
{
    public BasicChar whom;

    [Header("Organ descs")]
    [SerializeField]
    private TextMeshProUGUI Dicks = null;

    [SerializeField]
    private TextMeshProUGUI Balls = null, Vagina = null, Boobs = null;

    [Header("Sliders")]
    public MascSlider mascSlider;

    public FemiSlider femiSlider;
    public Slider ArousalSlider;
    public TextMeshProUGUI OrgasmCounter;

    public virtual void OnEnable()
    {
    }

    public virtual void OnDisable()
    {
        whom.SexStats.ArousalChangeEvent -= Arousal;
        SexualOrgan.SomethingChanged -= Organs;
    }

    public void Setup(BasicChar basicChar)
    {
        whom = basicChar;
        mascSlider.Init(whom);
        femiSlider.Init(whom);
        Organs();
        whom.SexStats.ArousalChangeEvent += Arousal;
        SexualOrgan.SomethingChanged += Organs;
    }

    private void Arousal()
    {
        ArousalSlider.value = whom.SexStats.ArousalSlider;
        OrgasmCounter.text = whom.SexStats.SessionOrgasm.ToString();
    }

    private void Organs()
    {
        Dicks.text = whom.DicksLook();
        Balls.text = whom.BallsLook();
        Boobs.text = whom.BoobsLook();
        Vagina.text = whom.VagsLook();
    }
}